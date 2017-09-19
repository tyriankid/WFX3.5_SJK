using Hidistro.Core;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using System;
using System.Collections;
using System.Web;

namespace Hidistro.ControlPanel.Store
{
    public static class HiAffiliation
    {
        public static void LoadPage()
        {
            string text = HiAffiliation.ReturnUrl();
          
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace("\n", "");
               
                HttpContext.Current.Response.Redirect(text);
            }
        }

        public static string ReturnUrl()
        {
            int currentMemberUserId = Globals.GetCurrentMemberUserId(false);
            string result;
            if (currentMemberUserId > 0)
            {
              
                MemberInfo currentMember = MemberProcessor.GetCurrentMember();
                if (currentMember != null)
                {
                    result = HiAffiliation.ReturnUrlByUser(currentMember);
                   
                    return result;
                }
            }
            result = HiAffiliation.ReturnUrlByQueryString();
          
            return result;
        }

        public static string ReturnUrlByQueryString()
        {
            int num = 0;
            string str = HttpContext.Current.Request.Url.PathAndQuery.ToString();
            string result;
          
            if (((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("returnUrl"))
            {
                result = string.Empty;
              
            }
            else if (HttpContext.Current.Request.Url.AbsolutePath == "/logout.aspx")
            {
                result = string.Empty;
               
            }
        
            else
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReferralId"]))
                {
                    num = Globals.RequestQueryNum("ReferralId");
                 
                    if (num > 0)
                    {
                        DistributorsInfo currentDistributors = DistributorsBrower.GetCurrentDistributors(num, true);
                        if (currentDistributors == null)
                        {
                          
                            HiAffiliation.SetReferralIdCookie("0", "", false);
                            result = "/Default.aspx?ReferralId=0";
                          
                            return result;
                        }
                        HttpCookie httpCookie = HttpContext.Current.Request.Cookies["Vshop-ReferralId"];
                     
                        if (httpCookie == null)
                        {
                            HiAffiliation.SetReferralIdCookie(num.ToString(), "", false);
                        }
                        else if (httpCookie != null && httpCookie.Value != num.ToString())
                        {
                            HiAffiliation.SetReferralIdCookie(num.ToString(), "", false);
                        }
                    }
                }
                else
                {
                   
                    HttpCookie httpCookie = HttpContext.Current.Request.Cookies["Vshop-ReferralId"];
                    if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
                    {
                        if (HttpContext.Current.Request.QueryString.Count > 0)
                        {
                            if (!((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("ReferralId"))
                            {
                                result = str + "&ReferralId=" + httpCookie.Value;
                               
                                return result;
                            }
                            result = string.Empty;
                           
                            return result;
                        }
                        else
                        {
                            if (!((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("ReferralId"))
                            {
                                result = str + "?ReferralId=" + httpCookie.Value;
                              
                                return result;
                            }
                            result = string.Empty;
                           
                            return result;
                        }
                    }
                }
                if (((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("returnUrl") || HttpContext.Current.Request.Url.AbsolutePath == "/logout.aspx")
                {
                   
                    result = string.Empty;
                   
                }
                else if (!((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("ReferralId") && HttpContext.Current.Request.QueryString.Count > 0)
                {
                    result = str + "&ReferralId=" + num.ToString();
                   
                }
                else if (!((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("ReferralId"))
                {
                    result = str + "?ReferralId=" + num.ToString();
                   
                }
                else
                {
                    result = string.Empty;
                }
            }
           
            return result;
        }

        /// <summary>
        /// 根据当前用户获取当前页是否跳转
        /// </summary>
        public static string ReturnUrlByUser(MemberInfo mInfo)
        {
            //设置当前用户访问的店铺CookieID
            DistributorsInfo currentDistributors = DistributorsBrower.GetCurrentDistributors(Globals.ToNum(mInfo.UserId), true);
            if (currentDistributors != null)
            {
                HiAffiliation.SetReferralIdCookie(currentDistributors.UserId.ToString(), "", false);
            }
            else
            {
                HiAffiliation.SetReferralIdCookie(mInfo.ReferralUserId.ToString(), "", false);
            }

            //设置跳转地址
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies["Vshop-ReferralId"];
            string result;
            string str = HttpContext.Current.Request.Url.PathAndQuery.ToString();
            if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReferralId"]))
                {
                    HiUriHelp hiUriHelp = new HiUriHelp(HttpContext.Current.Request.QueryString);
                    string queryString = hiUriHelp.GetQueryString("ReferralId");
                   
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        if (queryString == httpCookie.Value)
                        {
                            result = string.Empty;
                            return result;
                        }

                        //如果访问的店铺不是平台，但之前存储的店铺是平台时，强制更改【V20170906】
                        if (queryString != "0" && httpCookie.Value=="0")
                        {
                            int iReferralUserId = 0;
                            int.TryParse(queryString, out iReferralUserId);
                            MemberInfo currentMember = MemberProcessor.GetCurrentMember();
                            currentMember.ReferralUserId = iReferralUserId;
                            MemberProcessor.UpdateMember(currentMember);
                            HiAffiliation.SetReferralIdCookie(queryString, "", false);
                        }

                        hiUriHelp.SetQueryString("ReferralId", httpCookie.Value);
                        result = HttpContext.Current.Request.Url.AbsolutePath + hiUriHelp.GetNewQuery();
                      
                        return result;
                    }
                }
                if (((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("returnUrl"))
                {
                    result = string.Empty;
                }
                else if (HttpContext.Current.Request.Url.AbsolutePath == "/logout.aspx")
                {
                    result = string.Empty;
                }
                else if (!((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("ReferralId") && HttpContext.Current.Request.QueryString.Count > 0)
                {
                    result = str + "&ReferralId=" + httpCookie.Value;
                   
                }
                else if (!((IList)HttpContext.Current.Request.QueryString.AllKeys).Contains("ReferralId"))
                {
                    result = str + "?ReferralId=" + httpCookie.Value;
                  
                }
                else
                {
                    result = string.Empty;
                }
            }
            else
            {
                result = string.Empty;
            }
            return result;
        }

        public static string GetReturnUrl(string returnUrl)
        {
            if (returnUrl.IndexOf("?") > -1)
            {
                returnUrl = returnUrl.Substring(returnUrl.IndexOf("?"));
            }
            return returnUrl;
        }

        public static void SetReferralIdCookie(string referralId, string url = "", bool isRedirect = false)
        {
            Globals.ClearReferralIdCookie();
            Globals.SetDistributorCookie(Globals.ToNum(referralId));
            if (isRedirect && !string.IsNullOrEmpty(url))
            {
                HttpContext.Current.Response.Redirect(url);
            }
        }
    }
}
