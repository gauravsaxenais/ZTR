namespace ZTR.Framework.Security
{
    public class PasswordPolicyOptions
    {
        //// Password Policy Best practices
        //// https://pages.nist.gov/800-63-3/sp800-63b.html

        public PasswordPolicyOptions()
        {
            MinimumLength = 16;
        }

        /// <summary>
        /// Gets or sets the minimum length.
        /// </summary>
        /// <value>
        /// The minimum length.
        /// </value>
        public int MinimumLength { get; set; }
    }
}
