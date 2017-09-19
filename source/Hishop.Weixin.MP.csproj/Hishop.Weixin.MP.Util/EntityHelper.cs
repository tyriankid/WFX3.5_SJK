namespace Hishop.Weixin.MP.Util
{
    using Hishop.Weixin.MP.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    public static class EntityHelper
    {
        public static XDocument ConvertEntityToXml<T>(T entity) where T : class, new()
        {
            if (entity == null)
            {
            }
            entity = Activator.CreateInstance<T>();
            XDocument document = new XDocument();
            document.Add(new XElement("xml"));
            XElement root = document.Root;
            List<string> list = new string[] { "ToUserName", "FromUserName", "CreateTime", "MsgType", "Content", "ArticleCount", "Articles", "FuncFlag", "Title ", "Description ", "PicUrl", "Url", "Image" }.ToList<string>();
            Func<string, int> orderByPropName = new Func<string, int>(list.IndexOf);
            List<PropertyInfo> list2 = (from p in entity.GetType().GetProperties()
                                        orderby orderByPropName(p.Name)
                                        select p).ToList<PropertyInfo>();
            foreach (PropertyInfo info in list2)
            {
                string name = info.Name;
                switch (name)
                {
                    case "Articles":
                        {
                            XElement content = new XElement("Articles");
                            List<Article> list3 = info.GetValue(entity, null) as List<Article>;
                            foreach (Article article in list3)
                            {
                                IEnumerable<XElement> enumerable = ConvertEntityToXml<Article>(article).Root.Elements();
                                content.Add(new XElement("item", enumerable));
                            }
                            root.Add(content);
                            continue;
                        }
                    case "Image":
                        {
                            XElement element3 = new XElement("Image");
                            element3.Add(new XElement("MediaId", new XCData(((Image)info.GetValue(entity, null)).MediaId)));
                            root.Add(element3);
                            continue;
                        }
                    case "":
                        {
                            root.Add(new XElement(name, new XCData((info.GetValue(entity, null) as string) ?? "")));
                            continue;
                        }
                    default:
                        {
                            string str2 = info.PropertyType.Name;
                            if (str2 == null)
                            {
                                goto Label_0417;
                            }
                            if (!(str2 == "String"))
                            {
                                if (str2 == "DateTime")
                                {
                                    break;
                                }
                                if (str2 == "Boolean")
                                {
                                    goto Label_036A;
                                }
                                if (str2 == "ResponseMsgType")
                                {
                                    goto Label_03BD;
                                }
                                if (str2 == "Article")
                                {
                                    goto Label_03EA;
                                }
                                goto Label_0417;
                            }
                            root.Add(new XElement(name, new XCData((info.GetValue(entity, null) as string) ?? "")));
                            continue;
                        }
                }
                DateTime time = (DateTime)info.GetValue(entity, null);
                root.Add(new XElement(name, time.Ticks));
                continue;
            Label_036A:
                if (name == "FuncFlag")
                {
                    root.Add(new XElement(name, ((bool)info.GetValue(entity, null)) ? "1" : "0"));
                    continue;
                }
                goto Label_0417;
            Label_03BD:
                root.Add(new XElement(name, info.GetValue(entity, null).ToString().ToLower()));
                continue;
            Label_03EA:
                root.Add(new XElement(name, info.GetValue(entity, null).ToString().ToLower()));
                continue;
            Label_0417:
                root.Add(new XElement(name, info.GetValue(entity, null)));
            }
            return document;
        }

        public static void FillEntityWithXml<T>(T entity, XDocument doc) where T : AbstractRequest, new()
        {
            if (entity == null)
            {
            }
            entity = Activator.CreateInstance<T>();
            XElement root = doc.Root;
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                string name = info.Name;
                if (root.Element(name) != null)
                {
                    switch (info.PropertyType.Name)
                    {
                        case "DateTime":
                            {
                                info.SetValue(entity, new DateTime(long.Parse(root.Element(name).Value)), null);
                                continue;
                            }
                        case "Boolean":
                            {
                                if (!(name == "FuncFlag"))
                                {
                                    break;
                                }
                                info.SetValue(entity, root.Element(name).Value == "1", null);
                                continue;
                            }
                        case "Int64":
                            {
                                info.SetValue(entity, long.Parse(root.Element(name).Value), null);
                                continue;
                            }
                        case "Int32":
                            {
                                info.SetValue(entity, int.Parse(root.Element(name).Value), null);
                                continue;
                            }
                        case "RequestEventType":
                            {
                                info.SetValue(entity, EventTypeHelper.GetEventType(root.Element(name).Value), null);
                                continue;
                            }
                        case "RequestMsgType":
                            {
                                info.SetValue(entity, MsgTypeHelper.GetMsgType(root.Element(name).Value), null);
                                continue;
                            }
                    }
                    info.SetValue(entity, root.Element(name).Value, null);
                }
            }
        }
    }
}
