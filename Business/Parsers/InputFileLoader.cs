namespace Business.Parsers
{
    using EnsureThat;
    using Google.Protobuf;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public class InputFileLoader
    {
        public void GenerateFiles(string protoFileName, params string[] args)
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(protoFileName);

            bool deletePath = false;
            
            try
            {
                // try to use protoc
                protoFileName = GenerateCSharpFile(protoFileName, args);
                var message = GetAllMessages(protoFileName + "Power.cs");
                
                deletePath = true;
            }
            finally
            {
                if (deletePath)
                {
                    File.Delete(protoFileName);
                }
            }
        }

        public IMessage GetAllMessages(string path)
        {
            var assembly = Assembly.LoadFile(path);

            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.GetInterfaces().Contains(typeof(IMessage))
                                     && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as IMessage;

            foreach (var instance in instances)
            {
                if (instance.Descriptor.Name == "Config" && CanConvertToMessageType(instance.GetType()))
                {
                    return instance;
                }
            }

            return null;
        }

        public string GetProtocPath(out string folder)
        {
            const string Name = "protoc.exe";
            string lazyPath = CombinePathFromAppRoot(Name);

            if (File.Exists(lazyPath))
            {   // use protoc.exe from the existing location (faster)
                folder = null;
                return lazyPath;
            }

            folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, Name);
            
            // look inside ourselves...
            using (Stream resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                typeof(InputFileLoader).Namespace + "." + Name))
            using (Stream outFile = File.OpenWrite(path))
            {
                long len = 0;
                int bytesRead;
                byte[] buffer = new byte[4096];
                while ((bytesRead = resStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outFile.Write(buffer, 0, bytesRead);
                    len += bytesRead;
                }
                outFile.SetLength(len);
            }
            return path;
        }

        public string GenerateCSharpFile(string path, params string[] args)
        {
            string tmpOutputFolder = string.Empty;
            string tmpFolder = null, protocPath = null;
            try
            {
                tmpOutputFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
                Directory.CreateDirectory(tmpOutputFolder);

                protocPath = GetProtocPath(out tmpFolder);

                var protoFileLocation = @"F:\ZTR\Business\Parsers\ProtoFiles\Proto";
                
                var fileName = path;

                var psi = new ProcessStartInfo(
                    protocPath,
                    arguments: $" --include_imports --include_source_info --proto_path={protoFileLocation} --csharp_out={tmpOutputFolder} --error_format=gcc {fileName} {string.Join(" ", args)}"
                );
                
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.WorkingDirectory = Environment.CurrentDirectory;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = psi.RedirectStandardError = true;

                using (Process proc = Process.Start(psi))
                {
                    Thread errThread = new Thread(DumpStream(proc.StandardError));
                    Thread outThread = new Thread(DumpStream(proc.StandardOutput));
                    errThread.Name = "stderr reader";
                    outThread.Name = "stdout reader";
                    errThread.Start();
                    outThread.Start();
                    proc.WaitForExit();
                    outThread.Join();
                    errThread.Join();
                    if (proc.ExitCode != 0)
                    {
                        if (HasByteOrderMark(path))
                        {
                            //stderr.WriteLine("The input file should be UTF8 without a byte-order-mark (in Visual Studio use \"File\" -> \"Advanced Save Options...\" to rectify)");
                        }
                        throw new ProtoParseException(Path.GetFileName(path));
                    }
                    return tmpOutputFolder;
                }
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(tmpOutputFolder))
                {
                    try { Directory.Delete(tmpOutputFolder, true); }
                    catch { } // swallow
                }
                throw;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(tmpFolder))
                {
                    try { Directory.Delete(tmpFolder, true); }
                    catch { } // swallow
                }

            }
        }

        private bool HasByteOrderMark(string path)
        {
            try
            {
                using (Stream s = File.OpenRead(path))
                {
                    return s.ReadByte() > 127;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex); // log only
                return false;
            }
        }

        // <summary>
        // Called by NewtonSoft.Json's method to ask if this object can serialize
        // an object of a given type.
        // </summary>
        // <returns>True if the objectType is a Protocol Message.</returns>
        private bool CanConvertToMessageType(Type objectType)
        {
            return typeof(IMessage)
                .IsAssignableFrom(objectType);
        }

        private ThreadStart DumpStream(TextReader reader)
        {
            return (ThreadStart)delegate
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Debug.WriteLine(line);
                }
            };
        }

        private string CombinePathFromAppRoot(string path)
        {
            string loaderPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (!string.IsNullOrEmpty(loaderPath)
                && loaderPath[loaderPath.Length - 1] != Path.DirectorySeparatorChar
                && loaderPath[loaderPath.Length - 1] != Path.AltDirectorySeparatorChar)
            {
                loaderPath += Path.DirectorySeparatorChar;
            }
            if (loaderPath.StartsWith(@"file:\"))
            {
                loaderPath = loaderPath.Substring(6);
            }
            return Path.Combine(Path.GetDirectoryName(loaderPath), path);
        }
    }
    public sealed class ProtoParseException : Exception
    {
        public ProtoParseException(string file) : base("An error occurred parsing " + file) { }
    }
}
