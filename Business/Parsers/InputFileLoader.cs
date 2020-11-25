namespace Business.Parsers
{
    using EnsureThat;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    public class InputFileLoader
    {
        public void GenerateFiles(string path, params string[] args)
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(path);

            bool deletePath = false;

            try
            {
                // try to use protoc
                path = CompileProtoFile(path, args);
                deletePath = true;
            }
            finally
            {
                if (deletePath)
                {
                    File.Delete(path);
                }
            }
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

        public string CompileProtoFile(string path, params string[] args)
        {
            string tmp = Path.GetTempFileName();
            string tmpFolder = null, protocPath = null;
            try
            {
                protocPath = GetProtocPath(out tmpFolder);
                ProcessStartInfo psi = new ProcessStartInfo(
                    protocPath,
                    string.Format(@""" --include_imports --include_source_info --csharp_out={0}"" ""--proto_path={1}"" ""--proto_path={2}"" --error_format=gcc ""{3}"" {4}",
                             tmp, // output file
                             /*Environment.CurrentDirectory*/@"F:\ZTR\Business\Parsers\ProtoFiles\Proto", // primary search path
                             Path.GetDirectoryName(protocPath), // secondary search path
                             Path.Combine(/*Environment.CurrentDirectory*/@"F:\ZTR\Business\Parsers\ProtoFiles\Proto", path), // input file
                             string.Join(" ", args) // extra args
                    )
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
                    return tmp;
                }
            }
            catch
            {
                try { if (File.Exists(tmp)) File.Delete(tmp); }
                catch { } // swallow
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(tmpFolder))
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
