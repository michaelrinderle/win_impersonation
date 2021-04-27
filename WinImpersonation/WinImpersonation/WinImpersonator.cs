//    __ _/| _/. _  ._/__ /
// _\/_// /_///_// / /_|/
//            _/
// sof digital 2021
// written by michael rinderle <michael@sofdigital.net>

// mit license
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;

namespace WinImpersonation
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class WinImpersonator : IDisposable
    {
        #region WIN32_API

        protected const int LOGON32_PROVIDER_DEFAULT = 0;
        protected const int LOGON32_LOGON_INTERACTIVE = 2;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        #endregion

        #region PROPERTIES

        public WindowsIdentity WinIdentity;
        private IntPtr nAccessToken;

        #endregion

        #region CONSTRUCTORS

        public WinImpersonator()
        {
            this.WinIdentity = WindowsIdentity.GetCurrent();
        }

        public WinImpersonator(string username, string domain, string password)
        {
            Login(username, domain, password);
        }

        #endregion

        #region METHODS

        public bool Login(string username, string domain, string password)
        {
            try
            {
                if (domain == null)
                    domain = Environment.MachineName;

                FreeNativeResources();
                var loginResult = LogonUser(
                    username,
                    domain,
                    password,
                    LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT,
                    out this.nAccessToken);

                if (!loginResult)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine(error);
                    return false;
                }

                WinIdentity = new WindowsIdentity(this.nAccessToken);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool RunImpersonatedCode(Action action)
        {
            try
            {
#if NET461
                using (user.Impersonate())
#else
                WindowsIdentity.RunImpersonated(WinIdentity.AccessToken, () =>
#endif
                { action(); }
#if !NET461
                );
#endif
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void FreeNativeResources()
        {
            if (this.nAccessToken != IntPtr.Zero)
            {
                CloseHandle(nAccessToken);
                this.nAccessToken = IntPtr.Zero;

            }

            if (this.WinIdentity != null)
            {
                this.WinIdentity.Dispose();
                this.WinIdentity = null;
            }
        }

        void IDisposable.Dispose()
        {
            WinIdentity?.Dispose();
        }

        #endregion
    }
}
