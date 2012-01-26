using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Diagnostics;

namespace MCM_Common
{
    public class Utils
    {
        private Utils() { } // CA1053, http://msdn2.microsoft.com/library/ms182169(VS.90).aspx

        public static void Logger(string sText)
        {
            foreach (System.Windows.Forms.Form form in System.Windows.Forms.Application.OpenForms)
            {
                if (form.GetType().ToString() == "MediaCenterMaster.frmMain")
                {
                    try
                    {
                        MethodInfo logMethod = form.GetType().GetMethod("TextLog");
                        logMethod.Invoke(form, new object[] { sText });
                    }
                    catch { }
                }
            }
            return;
        }
        public static int CInt(Object oData)
        {
            int iRetVal = 0;

            if (oData == null)
                return -1;

            try
            {
                iRetVal = int.Parse(oData.ToString().Trim());
            }
            catch { }

            return iRetVal;
        }
        public static double CDbl(Object oData)
        {
            double iRetVal = 0.0;

            if (oData == null)
                return -1.0;

            try
            {
                string sTemp = oData.ToString().Trim().Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                iRetVal = double.Parse(sTemp);
            }
            catch { }

            return iRetVal;
        }
        public static string CStr(byte[] bData, int iCount)
        {
            string s = string.Empty;
            try { s = System.Text.Encoding.UTF8.GetString(bData, 0, iCount); } catch { }
            return s;
        }
        public static string FormatDollars(Object oData)
        {
            try
            {
                return MCM_Common.Utils.CDbl(oData).ToString("$#,##0.00;-$#,##0.00;\"\"");
            }
            catch { }
            return oData.ToString();
        }
        public static decimal UnformatDollars(Object oData)
        {
            try
            {
                string sTemp = oData.ToString();
                sTemp = sTemp.Replace("$", "");
                sTemp = sTemp.Replace(System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator, "");
                sTemp = CDbl(sTemp).ToString();
                return decimal.Parse(sTemp);
            }
            catch { }

            return 0.00M;
        }

        public static string GetAppSetting(string sKeyName)
        {
            string sVal = string.Empty; ;

            try
            {
                sVal = Utils.AppSettingKey.GetValue(sKeyName, string.Empty).ToString();
            }
            catch
            {
                sVal = "";
            }

            if (string.IsNullOrEmpty(sVal))
                return GetAppSettingOld(sKeyName);

            return sVal;
        }
        public static Microsoft.Win32.RegistryKey AppSettingKey
        {
            get
            {
                Microsoft.Win32.RegistryKey key = System.Windows.Forms.Application.UserAppDataRegistry;
                string sKeyToUse = key.ToString().Replace("HKEY_CURRENT_USER\\", "");
                sKeyToUse = sKeyToUse.Substring(0, sKeyToUse.LastIndexOf("\\"));
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sKeyToUse, true);

                return key;
            }
        }
        public static string GetAppSettingOld(string sKeyName)
        {
            string sVal = string.Empty;

            try
            {
                System.Configuration.Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                sVal = c.AppSettings.Settings[sKeyName].Value;
            }
            catch { }

            return sVal;
        }
        public static string Chopper(string sText, string sSearch, string sEnd, int offset)
        {
            string sIntermediate = "";

            try
            {
                if (string.IsNullOrEmpty(sSearch))
                {
                    sIntermediate = sText.Substring(offset);
                }
                else
                {
                    if (sText.Contains(sSearch) == false)
                        return sText;

                    sIntermediate = sText.Substring(sText.IndexOf(sSearch) + sSearch.Length + offset);
                }

                if (string.IsNullOrEmpty(sEnd))
                    return sIntermediate;

                if (sIntermediate.Contains(sEnd) == false)
                    return sIntermediate;

                return sIntermediate.Substring(0, sIntermediate.IndexOf(sEnd));
            }
            catch
            {
                if (sIntermediate == "")
                    return sText;
                return sIntermediate;
            }
        }
        public static string Chopper(string sText, string sSearch, string sEnd)
        {
            return MCM_Common.Utils.Chopper(sText, sSearch, sEnd, 0);
        }
        public static string ChopperBlank(string sText, string sSearch, string sEnd, int offset)
        {
            string sIntermediate = "";

            try
            {
                if (string.IsNullOrEmpty(sSearch))
                {
                    sIntermediate = sText.Substring(offset);
                }
                else
                {
                    if (sText.Contains(sSearch) == false)
                        return "";

                    sIntermediate = sText.Substring(sText.IndexOf(sSearch) + sSearch.Length + offset);
                }

                if (string.IsNullOrEmpty(sEnd))
                    return sIntermediate;

                if (sIntermediate.Contains(sEnd) == false)
                    return sIntermediate;

                return sIntermediate.Substring(0, sIntermediate.IndexOf(sEnd));
            }
            catch
            {
                if (sIntermediate == "")
                    return "";
                return sIntermediate;
            }
        }

        public static string ChopperBlank(string sText, string sSearch, string sEnd)
        {
            return MCM_Common.Utils.ChopperBlank(sText, sSearch, sEnd, 0);
        }

        public static string[] SubStrings(string sText, string sSplitter)
        {
            return sText.Split(new string[] { sSplitter }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static byte[] HexToBytes(string input)
        {
            string sCurrent = "";
            byte[] bOut = new byte[input.Length / 2];

            for (int i = 0; i < input.Length / 2; i++)
            {
                sCurrent = input[(i * 2)].ToString() + input[(i * 2) + 1].ToString();
                bOut[i] = byte.Parse(sCurrent, System.Globalization.NumberStyles.HexNumber);
            }

            return bOut;
        }
        public static System.Drawing.Bitmap LoadPictureFromURI(string url)
        {
            System.Net.HttpWebRequest wreq;
            System.Net.HttpWebResponse wresp;
            System.IO.Stream mystream;
            System.Drawing.Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = false;
                wreq.MaximumAutomaticRedirections = 1;
                wreq.Timeout = 10000;
                wreq.KeepAlive = false;

                wresp = (System.Net.HttpWebResponse)wreq.GetResponse();

                if (wresp.StatusCode == HttpStatusCode.OK)
                    if ((mystream = wresp.GetResponseStream()) != null)
                        bmp = new System.Drawing.Bitmap(mystream);
            }
            catch { }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }

            return (bmp);
        }

        public static string PageFetch(string sURL)
        {
            WebRequest oReq;
            HttpWebRequest ohReq;
            string sContent = "";

            try
            {
                oReq = WebRequest.Create(sURL);
                ohReq = (HttpWebRequest)oReq;

                ohReq.Accept = "*/*";
                ohReq.Referer = sURL;
                oReq.Headers.Add("Accept-Language: ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3");
                oReq.Headers.Add("Accept-Encoding: plain");
                oReq.Headers.Add("Accept-Charset: windows-1251,utf-8;q=0.7,*;q=0.7");
                ohReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET CLR 1.1.4322)";
                ohReq.Connection = "false";
                oReq.Headers.Add("Pragma: no-cache");
                oReq.Method = "GET";
                oReq.Timeout = 10000;
                try
                {
                    ohReq.Headers.Add("Host: " + Chopper(sURL, "://", "/"));
                }
                catch { }
                ohReq.MaximumAutomaticRedirections = 3;
                try
                {
                    HttpWebResponse oResp = (HttpWebResponse)oReq.GetResponse();
                    
                    string charSet = oResp.CharacterSet;
                    StreamReader oSRead = new StreamReader(oResp.GetResponseStream(), System.Text.Encoding.GetEncoding(charSet));

                    sContent = oSRead.ReadToEnd();

                } catch (WebException)
                {
                    
                }
                
            }
            catch (TimeoutException)
            {
                return "[timeout]";
            }
            catch (Exception ex)
            {
                //Logger("MCM_Common.Utils.PageFetch() exception:\r\n" + ex.Message + "\r\nAt URL: " + sURL);
                return "[exception: " + ex.Message + "]";
            }

            return sContent;
        }

        public static string ProgramDataFolder
        {
            get
            {
                string sPathReturn = "";

                if (Utils.GetAppSetting("OverrideProgramDataFolder") != "")
                    return Utils.GetAppSetting("OverrideProgramDataFolder");

                try
                {
                    string sPath = System.Environment.GetEnvironmentVariable("ProgramData").Trim();
                    if (sPath.Length > 0)
                        sPathReturn = sPath;
                }
                catch { }
                return sPathReturn;
            }
        }

        public static string SafeDownloadFilename(string sFilename)
        {
            sFilename = sFilename.Replace("/", "-");
            sFilename = sFilename.Replace("\\", "-");
            sFilename = sFilename.Replace(": ", ", ");
            sFilename = sFilename.Replace(":", "-");
            sFilename = sFilename.Replace("*", "-");
            sFilename = sFilename.Replace("|", "-");
            sFilename = sFilename.Replace("\"", "''");
            sFilename = sFilename.Replace("?", "_");
            return sFilename;
        }

        public static string ProperCase(string stringInput)
        {
            return Utils.ProperCase(stringInput, true);
        }
        public static string ProperCase(string stringInput, bool bLowerOthers)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool fEmptyBefore = true;
            foreach (char ch in stringInput)
            {
                char chThis = ch;
                if (Char.IsWhiteSpace(chThis) || chThis == '.')
                    fEmptyBefore = true;
                else
                {
                    if (Char.IsLetter(chThis) && fEmptyBefore)
                        chThis = Char.ToUpper(chThis);
                    else
                        chThis = (bLowerOthers) ? Char.ToLower(chThis) : chThis;
                    fEmptyBefore = false;
                }
                sb.Append(chThis);
            }
            return sb.ToString();
        }

        public static string AppFolder
        {
            get
            {
                try
                {
                    return System.Windows.Forms.Application.UserAppDataPath.Substring(0, System.Windows.Forms.Application.UserAppDataPath.LastIndexOf("\\"));
                }
                catch
                {
                    try
                    {
                        return System.Windows.Forms.Application.ExecutablePath.Substring(0, System.Windows.Forms.Application.ExecutablePath.LastIndexOf("\\"));
                    }
                    catch
                    {
                        try
                        {
                            return System.Environment.CurrentDirectory;
                        }
                        catch
                        {
                            try
                            {
                                return System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            }
                            catch
                            {
                                return ".";
                            }
                        }
                    }
                }
            }
        }


        public static string TrueAppFolder
        {
            get
            {
                try
                {
                    return System.Windows.Forms.Application.ExecutablePath.Substring(0, System.Windows.Forms.Application.ExecutablePath.LastIndexOf("\\"));
                }
                catch
                {
                    try
                    {
                        return System.Environment.CurrentDirectory;
                    }
                    catch
                    {
                        try
                        {
                            return System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        }
                        catch
                        {
                            return ".";
                        }
                    }
                }
            }
        }

        public static string SafeYear(Object oYear)
        {
            try
            {
                if (oYear.GetType() == typeof(int))
                {
                    if (((int)oYear) < 100)
                        return (1900 + ((int)oYear)).ToString();
                    return ((int)oYear).ToString();
                }

                if (oYear.GetType() == typeof(string))
                {
                    if (((string)oYear).Length <= 4)
                        return (string)oYear;

                    if (((string)oYear).Contains("/"))
                        return ((string)oYear).Substring(0, 4);

                    return DateTime.Parse((string)oYear).Year.ToString();
                }
            }
            catch { }

            return "1900";
        }

        public static string StripHTML(string sText)
        {
            return System.Text.RegularExpressions.Regex.Replace(sText, @"<(.|\n)*?>", string.Empty).Trim();
        }

        public static string StripYearAndComments(string sText)
        {
            return System.Text.RegularExpressions.Regex.Replace(sText, @"\((.|\n)*?\)", string.Empty).Trim();
        }

        public static string UnHTML(string sIn)
        {
            sIn = sIn.Replace("&amp;", "&");
            sIn = sIn.Replace("&apos;", "'");
            sIn = sIn.Replace("&quot;", "\"");
            sIn = sIn.Replace("&lt;", "<");
            sIn = sIn.Replace("&gt;", ">");

            while (sIn.Contains("&#x"))
            {
                int iPos = sIn.IndexOf("&#x");
                if (sIn.Substring(iPos + 5, 1) != ";")
                    break;
                string sCode = sIn.Substring(iPos + 3, 2);
                byte[] bByteVersion = Utils.HexToBytes(sCode);
                string sCharToReplaceWith = ((Char)bByteVersion[0]).ToString();
                sIn = sIn.Replace("&#x" + sCode + ";", sCharToReplaceWith);
            }

            while (sIn.Contains("&#"))
            {
                string sCode = MCM_Common.Utils.Chopper(sIn, "&#", ";");
                byte bByteVersion = (Byte)(MCM_Common.Utils.CInt(sCode));
                string sCharToReplaceWith = ((Char)bByteVersion).ToString();
                sIn = sIn.Replace("&#" + sCode + ";", sCharToReplaceWith);
            }

            sIn = sIn.Replace("\"", "");
            sIn = sIn.Replace("&amp;", "&");
            sIn = sIn.Replace("&apos;", "'");
            sIn = sIn.Replace("&quot;", "\"");
            sIn = sIn.Replace("&lt;", "<");
            sIn = sIn.Replace("&gt;", ">");

            return sIn;
        }

        public static int EndAt(string sWorkingTitle, string sSearch, int iCurrentEndAt)
        {
            if (sWorkingTitle.IndexOf("720p") < iCurrentEndAt)
                return sWorkingTitle.IndexOf("720p");
            return iCurrentEndAt;
        }

        public static string PreprocessFile(string sTitle)
        {
            string sWorkingTitle = sTitle.ToLower();
            int iEndAt = sWorkingTitle.Length;

            iEndAt = Utils.EndAt(sWorkingTitle, " dvd", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, ".dvd", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "[dvd", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "]dvd", iEndAt);

            iEndAt = Utils.EndAt(sWorkingTitle, " r5", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, ".r5", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "[r5", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "]r5", iEndAt);

            iEndAt = Utils.EndAt(sWorkingTitle, " ts", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, ".ts", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "[ts", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "]ts", iEndAt);

            iEndAt = Utils.EndAt(sWorkingTitle, " ws", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, ".ws", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "[ws", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "]ws", iEndAt);

            iEndAt = Utils.EndAt(sWorkingTitle, "[direct", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "[eng", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "[extend", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "[unrate", iEndAt);

            iEndAt = Utils.EndAt(sWorkingTitle, "1080p", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "720p", iEndAt);

            iEndAt = Utils.EndAt(sWorkingTitle, "ac-3", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "ac3", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "bdrip", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "bluray", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "dvdrip", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "final edition", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "fxg", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "fxm", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "hdrip", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "hd-rip", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "mp4", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "proper", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "repack", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "subbed", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "tvrip", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "unrated", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "x264", iEndAt);
            iEndAt = Utils.EndAt(sWorkingTitle, "xvid", iEndAt);

            return sWorkingTitle.Substring(0, iEndAt);
        }

        public static void ConvertFolderToTitleAndYear(string sFolder, out string sTitle, out string sYear)
        {
            Utils.ConvertFolderToTitleAndYear(sFolder, out sTitle, out sYear, false);
            return;
        }
        public static void ConvertFolderToTitleAndYear(string sFolder, out string sTitle, out string sYear, bool bWantDownloadStyle)
        {
            string sTitleToSearch = sFolder;
            string sYearToSearch = "";

            try
            {
                if (Utils.GetAppSetting("MoreRelaxedSearching") != "False")
                    sTitleToSearch = Utils.PreprocessFile(sTitleToSearch);

                sTitleToSearch = Utils.MapInternationals(sTitleToSearch);

                if (bWantDownloadStyle)
                    sTitleToSearch = sTitleToSearch.Replace(".", " ").Replace("  ", " ");
                else
                    sTitleToSearch = sTitleToSearch.Replace("  ", " ");

                if (sTitleToSearch.Contains(" 1 - "))
                    sTitleToSearch = sTitleToSearch.Replace(" 1 - ", " - ");
                else if (sTitleToSearch.Contains(" 1 [19"))
                    sTitleToSearch = sTitleToSearch.Replace(" 1 [19", " [19");
                else if (sTitleToSearch.Contains(" 1 (19"))
                    sTitleToSearch = sTitleToSearch.Replace(" 1 (19", " (19");
                else if (sTitleToSearch.Contains(" 1 [20"))
                    sTitleToSearch = sTitleToSearch.Replace(" 1 [20", " [20");
                else if (sTitleToSearch.Contains(" 1 (20"))
                    sTitleToSearch = sTitleToSearch.Replace(" 1 (20", " (20");

                if (sTitleToSearch.Contains("[19"))
                {
                    sYearToSearch = sTitleToSearch.Substring(sTitleToSearch.IndexOf("[19")).Replace(" ", "").Replace("[", "").Replace("]", "");
                    sTitleToSearch = sTitleToSearch.Substring(0, sTitleToSearch.IndexOf("[19"));
                }
                else if (sTitleToSearch.Contains("(19"))
                {
                    sYearToSearch = sTitleToSearch.Substring(sTitleToSearch.IndexOf("(19")).Replace(" ", "").Replace("(", "").Replace(")", "");
                    sTitleToSearch = sTitleToSearch.Substring(0, sTitleToSearch.IndexOf("(19"));
                }
                else if (sTitleToSearch.Contains("[20"))
                {
                    sYearToSearch = sTitleToSearch.Substring(sTitleToSearch.IndexOf("[20")).Replace(" ", "").Replace("[", "").Replace("]", "");
                    sTitleToSearch = sTitleToSearch.Substring(0, sTitleToSearch.IndexOf("[20"));
                }
                else if (sTitleToSearch.Contains("(20"))
                {
                    sYearToSearch = sTitleToSearch.Substring(sTitleToSearch.IndexOf("(20")).Replace(" ", "").Replace("(", "").Replace(")", "");
                    sTitleToSearch = sTitleToSearch.Substring(0, sTitleToSearch.IndexOf("(20"));
                }
                else if (bWantDownloadStyle)
                {
                    if (sTitleToSearch.Contains("19"))
                    {
                        try
                        {
                            if (Char.IsDigit(sTitleToSearch.Substring(sTitleToSearch.IndexOf("19") + 3, 1).ToCharArray()[0]))
                            {
                                if (Char.IsDigit(sTitleToSearch.Substring(sTitleToSearch.IndexOf("19") + 4, 1).ToCharArray()[0]))
                                {
                                    sYearToSearch = sTitleToSearch.Substring(sTitleToSearch.IndexOf("19"), 5).Replace(" ", "");
                                    sTitleToSearch = sTitleToSearch.Substring(0, sTitleToSearch.IndexOf("19"));
                                }
                            }
                        }
                        catch { }
                    }
                    else if (sTitleToSearch.Contains("20"))
                    {
                        try
                        {
                            if (Char.IsDigit(sTitleToSearch.Substring(sTitleToSearch.IndexOf("20") + 3, 1).ToCharArray()[0]))
                            {
                                if (Char.IsDigit(sTitleToSearch.Substring(sTitleToSearch.IndexOf("20") + 4, 1).ToCharArray()[0]))
                                {
                                    sYearToSearch = sTitleToSearch.Substring(sTitleToSearch.IndexOf("20"), 5).Replace(" ", "");
                                    sTitleToSearch = sTitleToSearch.Substring(0, sTitleToSearch.IndexOf("20"));
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            sTitle = sTitleToSearch;
            sYear = sYearToSearch;

            if (sYear == "")
            {
                try
                {
                    for (int i = 1900; i <= 2050; i++)
                    {
                        if (bWantDownloadStyle)
                        {
                            if (sTitle.Contains(i.ToString()))
                            {
                                sTitle = sTitle.Substring(0, sTitle.IndexOf(i.ToString()));
                                sYear = i.ToString();
                                break;
                            }
                        }
                        else
                        {
                            if (sTitle.Contains("(" + i.ToString() + ")"))
                            {
                                sTitle = sTitle.Substring(0, sTitle.IndexOf(i.ToString()));
                                sYear = i.ToString();
                                break;
                            }
                            else if (sTitle.Contains("[" + i.ToString() + "]"))
                            {
                                sTitle = sTitle.Substring(0, sTitle.IndexOf(i.ToString()));
                                sYear = i.ToString();
                                break;
                            }
                        }
                    }
                }
                catch { }
            }

            sTitle = Utils.ProperCase(sTitle.Trim());
            sYear = Utils.SafeYear(sYear.Trim());

            if ((sTitle == string.Empty) && (sYear != string.Empty))
            {
                sTitle = sYear;
                sYear = string.Empty;
            }

            if (sTitle.EndsWith(" ("))
                sTitle = sTitle.Substring(0, sTitle.Length - 2);

            return;
        }

        public static bool CompareMovieTitles(string sTitle1, string sTitle2)
        {
            sTitle1 = Utils.UnHTML(FixTitleForSearching(sTitle1)) + " ";
            sTitle2 = Utils.UnHTML(FixTitleForSearching(sTitle2)) + " ";

            sTitle1 = sTitle1.Replace(" 1 ", " i ");
            sTitle1 = sTitle1.Replace(" 2 ", " ii ");
            sTitle1 = sTitle1.Replace(" 3 ", " iii ");
            sTitle1 = sTitle1.Replace(" 4 ", " iv ");
            sTitle1 = sTitle1.Replace(" 5 ", " v ");
            sTitle1 = sTitle1.Replace(" 6 ", " vi ");
            sTitle1 = sTitle1.Replace(" 7 ", " vii ");
            sTitle1 = sTitle1.Replace(" 8 ", " viii ");
            sTitle1 = sTitle1.Replace(" 9 ", " ix ");
            sTitle1 = sTitle1.Replace(" 10 ", " x ");

            sTitle2 = sTitle2.Replace(" 1 ", " i ");
            sTitle2 = sTitle2.Replace(" 2 ", " ii ");
            sTitle2 = sTitle2.Replace(" 3 ", " iii ");
            sTitle2 = sTitle2.Replace(" 4 ", " iv ");
            sTitle2 = sTitle2.Replace(" 5 ", " v ");
            sTitle2 = sTitle2.Replace(" 6 ", " vi ");
            sTitle2 = sTitle2.Replace(" 7 ", " vii ");
            sTitle2 = sTitle2.Replace(" 8 ", " viii ");
            sTitle2 = sTitle2.Replace(" 9 ", " ix ");
            sTitle2 = sTitle2.Replace(" 10 ", " x ");

            return (sTitle1.Trim() == sTitle2.Trim());
        }

        public static string FixTitleForSearching(string sTitle)
        {
            sTitle = Utils.UnHTML(Utils.MapInternationals(sTitle).ToLower());
            sTitle = System.Text.RegularExpressions.Regex.Replace(sTitle, @"\(.*?\)", "").Trim();
            sTitle = sTitle + " ";
            sTitle = sTitle.Replace("_", " ");
            if (sTitle.StartsWith("the ")) sTitle = sTitle.Substring(4);
            if (sTitle.StartsWith("a ")) sTitle = sTitle.Substring(2);
            if (sTitle.StartsWith("an ")) sTitle = sTitle.Substring(3);
            if (sTitle.StartsWith("and ")) sTitle = sTitle.Substring(4);
            sTitle = sTitle.Replace(" the ", " ");
            sTitle = sTitle.Replace(" a ", " ");
            sTitle = sTitle.Replace(" an ", " ");
            sTitle = sTitle.Replace(" and ", " ");
            sTitle = sTitle.Replace(" of ", " ");
            sTitle = sTitle.Replace("!", " ");
            sTitle = sTitle.Replace(": ", " ");
            sTitle = sTitle.Replace(":", "");
            sTitle = sTitle.Replace("\"", " ");
            sTitle = sTitle.Replace("&", " ");
            sTitle = sTitle.Replace("²", " 2");
            sTitle = sTitle.Replace("³", " 3");
            sTitle = sTitle.Replace("·", "-");
            sTitle = sTitle.Replace("+", " ");
            sTitle = sTitle.Replace("'", "");
            sTitle = sTitle.Replace("-", " ");
            sTitle = sTitle.Replace("?", " ");
            sTitle = sTitle.Replace("!", " ");
            sTitle = sTitle.Replace(".", "");
            sTitle = sTitle.Replace("/", " ");
            sTitle = sTitle.Replace(", ", " ");
            sTitle = sTitle.Replace(",", "");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            return sTitle.Trim();
        }

        public static string FixTitleForSearching_KeepStopWords(string sTitle)
        {
            sTitle = sTitle.ToLower();
            sTitle = System.Text.RegularExpressions.Regex.Replace(sTitle, @"\(.*?\)", "").Trim();
            sTitle = sTitle + " ";
            sTitle = sTitle.Replace("!", " ");
            sTitle = sTitle.Replace(": ", " ");
            sTitle = sTitle.Replace(":", "");
            sTitle = sTitle.Replace("\"", " ");
            sTitle = sTitle.Replace("&", " ");
            sTitle = sTitle.Replace("+", " ");
            sTitle = sTitle.Replace("'", "");
            sTitle = sTitle.Replace("-", " ");
            sTitle = sTitle.Replace("_", " ");
            sTitle = sTitle.Replace("/", " ");
            sTitle = sTitle.Replace(", ", " ");
            sTitle = sTitle.Replace(",", "");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            return sTitle.Trim();
        }

        public static bool CompareMovieTitlesLoose(string sTitle1, string sTitle2)
        {
            sTitle1 = FixTitleForSearchingLoose(sTitle1) + " ";
            sTitle2 = FixTitleForSearchingLoose(sTitle2) + " ";

            sTitle1 = sTitle1.Replace(" 1 ", " i ");
            sTitle1 = sTitle1.Replace(" 2 ", " ii ");
            sTitle1 = sTitle1.Replace(" 3 ", " iii ");
            sTitle1 = sTitle1.Replace(" 4 ", " iv ");
            sTitle1 = sTitle1.Replace(" 5 ", " v ");
            sTitle1 = sTitle1.Replace(" 6 ", " vi ");
            sTitle1 = sTitle1.Replace(" 7 ", " vii ");
            sTitle1 = sTitle1.Replace(" 8 ", " viii ");
            sTitle1 = sTitle1.Replace(" 9 ", " ix ");
            sTitle1 = sTitle1.Replace(" 10 ", " x ");

            sTitle2 = sTitle2.Replace(" 1 ", " i ");
            sTitle2 = sTitle2.Replace(" 2 ", " ii ");
            sTitle2 = sTitle2.Replace(" 3 ", " iii ");
            sTitle2 = sTitle2.Replace(" 4 ", " iv ");
            sTitle2 = sTitle2.Replace(" 5 ", " v ");
            sTitle2 = sTitle2.Replace(" 6 ", " vi ");
            sTitle2 = sTitle2.Replace(" 7 ", " vii ");
            sTitle2 = sTitle2.Replace(" 8 ", " viii ");
            sTitle2 = sTitle2.Replace(" 9 ", " ix ");
            sTitle2 = sTitle2.Replace(" 10 ", " x ");

            return (sTitle1.Trim() == sTitle2.Trim());
        }

        public static string FixTitleForSearchingLoose(string sTitle)
        {
            sTitle = Utils.UnHTML(Utils.MapInternationals(sTitle).ToLower());
            sTitle = System.Text.RegularExpressions.Regex.Replace(sTitle, @"\(*?\)", "").Trim();
            sTitle = sTitle + " ";
            sTitle = sTitle.Replace("_", " ");
            if (sTitle.StartsWith("the ")) sTitle = sTitle.Substring(4);
            if (sTitle.StartsWith("a ")) sTitle = sTitle.Substring(2);
            if (sTitle.StartsWith("an ")) sTitle = sTitle.Substring(3);
            if (sTitle.StartsWith("and ")) sTitle = sTitle.Substring(4);
            sTitle = sTitle.Replace(" the ", " ");
            sTitle = sTitle.Replace(" a ", " ");
            sTitle = sTitle.Replace(" an ", " ");
            sTitle = sTitle.Replace(" and ", " ");
            sTitle = sTitle.Replace(" of ", " ");
            sTitle = sTitle.Replace("!", " ");
            sTitle = sTitle.Replace("\"", " ");
            sTitle = sTitle.Replace("&", " ");
            sTitle = sTitle.Replace("²", " 2");
            sTitle = sTitle.Replace("³", " 3");
            sTitle = sTitle.Replace("·", "-");
            sTitle = sTitle.Replace("+", " ");
            sTitle = sTitle.Replace("'", "");
            sTitle = sTitle.Replace("-", " ");
            sTitle = sTitle.Replace("?", " ");
            sTitle = sTitle.Replace("!", " ");
            sTitle = sTitle.Replace(".", "");
            sTitle = sTitle.Replace("/", " ");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            sTitle = sTitle.Replace("  ", " ");
            return sTitle.Trim();
        }

        public static string MapInternationals(string sText)
        {
            sText = sText.Replace("À", "A");
            sText = sText.Replace("Â", "A");
            sText = sText.Replace("Ã", "A");
            sText = sText.Replace("Ä", "A");
            sText = sText.Replace("Å", "A");
            sText = sText.Replace("Æ", "Ae");
            sText = sText.Replace("Ç", "C");
            sText = sText.Replace("È", "E");
            sText = sText.Replace("É", "E");
            sText = sText.Replace("Ê", "E");
            sText = sText.Replace("Ë", "E");
            sText = sText.Replace("Ì", "I");
            sText = sText.Replace("Í", "I");
            sText = sText.Replace("Î", "I");
            sText = sText.Replace("Ï", "I");
            sText = sText.Replace("Ð", "D");
            sText = sText.Replace("Ñ", "N");
            sText = sText.Replace("Ò", "O");
            sText = sText.Replace("Ó", "O");
            sText = sText.Replace("Ô", "O");
            sText = sText.Replace("Õ", "O");
            sText = sText.Replace("Ö", "O");
            sText = sText.Replace("×", "x");
            sText = sText.Replace("Ø", "O");
            sText = sText.Replace("Ù", "U");
            sText = sText.Replace("Ú", "U");
            sText = sText.Replace("Û", "U");
            sText = sText.Replace("Ü", "U");
            sText = sText.Replace("Ý", "Y");
            sText = sText.Replace("ß", "B");
            sText = sText.Replace("à", "a");
            sText = sText.Replace("á", "a");
            sText = sText.Replace("â", "a");
            sText = sText.Replace("ã", "a");
            sText = sText.Replace("ä", "a");
            sText = sText.Replace("å", "a");
            sText = sText.Replace("æ", "ae");
            sText = sText.Replace("ç", "c");
            sText = sText.Replace("è", "e");
            sText = sText.Replace("é", "e");
            sText = sText.Replace("ê", "e");
            sText = sText.Replace("ë", "e");
            sText = sText.Replace("ì", "i");
            sText = sText.Replace("í", "i");
            sText = sText.Replace("î", "i");
            sText = sText.Replace("ï", "i");
            sText = sText.Replace("ð", "s");
            sText = sText.Replace("ñ", "n");
            sText = sText.Replace("ò", "o");
            sText = sText.Replace("ó", "o");
            sText = sText.Replace("ô", "o");
            sText = sText.Replace("õ", "o");
            sText = sText.Replace("ö", "o");
            sText = sText.Replace("ø", "o");
            sText = sText.Replace("ù", "u");
            sText = sText.Replace("ú", "u");
            sText = sText.Replace("û", "u");
            sText = sText.Replace("ü", "u");
            sText = sText.Replace("ý", "y");
            sText = sText.Replace("ÿ", "y");

            return sText;
        }

        public static bool IsPoster(System.Drawing.Image image)
        {
            try
            {
                System.Drawing.Imaging.PixelFormat x = image.PixelFormat; // the PNG is in 32 BPP with alpha channel
                return (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb); // JPG's will not be
            }
            catch { }
            return false;
        }

        public static string GetRuntime(string sTMDB_Runtime, string sIMDB_Runtime)
        {
            string sRuntime = "";

            try
            {
                sRuntime = sTMDB_Runtime;
                if (sIMDB_Runtime != "")
                    sRuntime = sIMDB_Runtime;

                sRuntime = sRuntime.ToLower().Trim();

                // handle "1 hrs. 30 mins." format
                if (sRuntime.Contains("hrs.") && sRuntime.Contains("mins"))
                {
                    int iHrs = MCM_Common.Utils.CInt(sRuntime.Substring(0, 1));
                    int iMins = MCM_Common.Utils.CInt(sRuntime.Substring(7, 2).Trim());
                    sRuntime = ((iHrs * 60) + iMins).ToString();
                }

                // handle "US:90mins" format
                if (sRuntime.Contains(":")) sRuntime = sRuntime.Substring(sRuntime.IndexOf(":") + 1).Trim();

                // strip off "90 minutes" to just "90"
                if (sRuntime.Contains(" ")) sRuntime = sRuntime.Substring(0, sRuntime.IndexOf(" "));
                return sRuntime;
            }
            catch
            {
                return "";
            }
        }

        public static byte[] SerializeBitmap(System.Drawing.Bitmap bmp)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] b = ms.ToArray();
                ms.Dispose();
                return b;
            }
            catch { }

            return new byte[] { };
        }

        public static string SerializeObject(object oMovieInfo)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(oMovieInfo.GetType());
                MemoryStream ms = new MemoryStream();
                x.Serialize(ms, oMovieInfo);
                string s = Utils.CStr(ms.ToArray(), (int)ms.Length);
                ms.Dispose();
                return s;
            }
            catch (Exception ex)
            {
                MCM_Common.Utils.Logger(GetAllErrorDetails(ex));
            }

            return string.Empty;
        }

        public static string GetAllErrorDetails(Exception e)
        {
            return GetAllErrorDetails(e, new System.Collections.Generic.List<object>());
        }
        public static string GetAllErrorDetails(Exception e, System.Collections.Generic.List<object> oParams)
        {
            String sTXTError = "";

            if (Exception.Equals(e.GetBaseException(), e) == false)
            {
                sTXTError += GetAllErrorDetails(e.GetBaseException());
                sTXTError += "\r\n";
                sTXTError += "\r\n";
            }

            sTXTError += "-----------------------------------------------------------\r\n";
            sTXTError += "[EXCEPTION] " + e.GetType().ToString() + "\r\n";
            if (!string.IsNullOrEmpty(e.Message))
                sTXTError += "[MESSAGE]   " + e.Message + "\r\n";

            try
            {
                if ((!string.IsNullOrEmpty(e.StackTrace)) && (e.TargetSite != null))
                {
                    string sTemp = string.Empty;
                    foreach (string s in e.StackTrace.TrimEnd().Replace(" in c:\\", " in: C:\\").Split(new string[] { "\r\n" }, StringSplitOptions.None))
                    {
                        if (s.Contains(".cs:line "))
                        {
                            sTemp = s.TrimEnd().Replace(".cs:line ", ".cs (line ") + ")";
                            break;
                        }
                        else if (s.Contains(".vb:line "))
                        {
                            sTemp = s.TrimEnd().Replace(".vb:line ", ".vb (line ") + ")";
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(sTemp))
                    {
                        StackTrace stTrace = new StackTrace(e, true);
                        StackFrame[] stFrames = stTrace.GetFrames();

                        if (sTemp.Contains("\\" + System.Windows.Forms.Application.ProductName + "\\"))
                            sTemp = "/" + System.Windows.Forms.Application.ProductName + "/" + sTemp.Substring(sTemp.LastIndexOf("\\" + System.Windows.Forms.Application.ProductName + "\\") + ("\\" + System.Windows.Forms.Application.ProductName + "\\").Length).Replace("\\", "/");
                        sTXTError += "[FILE]      " + sTemp.Trim() + "\r\n";

                        if ((e.TargetSite.Attributes & System.Reflection.MethodAttributes.Private) > 0)
                            sTXTError += "[METHOD]    this." + e.TargetSite.Name + "()\r\n";
                        else if ((e.TargetSite.Attributes & System.Reflection.MethodAttributes.Static) > 0)
                            sTXTError += "[METHOD]    " + stFrames[stFrames.Length - 1].GetMethod().ReflectedType.Namespace + "." + e.TargetSite.Name + "()\r\n";
                        else
                            sTXTError += "[METHOD]    " + e.TargetSite.Name + "()\r\n";

                        try
                        {
                            if (oParams.Count > 0)
                            {
                                System.Reflection.ParameterInfo[] pParams = e.TargetSite.GetParameters();
                                if (pParams.Length > 0)
                                {
                                    for (int i = 0; i < pParams.Length; i++)
                                    {
                                        sTXTError += "[PARAMETER] " + pParams[i].ParameterType.ToString() + " " + pParams[i].Name + " = " + oParams[i].ToString() + "\r\n";
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            sTXTError += "-----------------------------------------------------------\r\n";

            try
            {
                if (Exception.Equals(e.InnerException, null) == false)
                {
                    sTXTError += "[INNER EXCEPTION]\r\n";
                    sTXTError += GetAllErrorDetails(e.InnerException);
                    sTXTError += "-----------------------------------------------------------\r\n";
                }
            }
            catch { }

            try
            {
                if (e.TargetSite != null)
                {
                    sTXTError += "[METHOD]\r\n";
                    sTXTError += "Name:       " + e.TargetSite.Name + "()\r\n";
                    sTXTError += "Module:     " + e.TargetSite.Module.Name + "\r\n";
                    sTXTError += "Attributes: " + e.TargetSite.Attributes.ToString() + "\r\n";
                    sTXTError += "-----------------------------------------------------------\r\n";
                }
            }
            catch { }

            try
            {
                if (e.Data != null)
                {
                    if (e.Data.Count > 0)
                    {
                        sTXTError += "[DATA]\r\n";
                        System.Collections.IEnumerator ienum = e.Data.GetEnumerator();
                        while (ienum.MoveNext())
                            sTXTError += ienum.Current.ToString() + "\r\n";
                        sTXTError += "-----------------------------------------------------------\r\n";
                    }
                }
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(e.StackTrace))
                {
                    sTXTError += "[STACK TRACE]\r\n";
                    string sTemp = string.Empty;
                    foreach (string s in e.StackTrace.TrimEnd().Replace(" in c:\\", " in: C:\\").Split(new string[] { "\r\n" }, StringSplitOptions.None ))
                    {
                        if (s.Contains(".cs:line "))
                            sTemp += s.TrimEnd().Replace(".cs:line ", ".cs (line ") + ")\r\n";
                        else if (s.Contains(".vb:line "))
                            sTemp += s.TrimEnd().Replace(".vb:line ", ".vb (line ") + ")\r\n";
                        else
                            sTemp += s.TrimEnd() + "\r\n";
                    }
                    sTXTError += sTemp;
                    sTXTError += "-----------------------------------------------------------\r\n";
                }
            }
            catch { }

            return sTXTError;
        }
    }
}