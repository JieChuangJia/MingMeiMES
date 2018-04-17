/******************************************************************************
 * 版权所有: Copyright (C) 沈阳新松机器人自动化股份有限公司
 * 文件名称:
 * 内容摘要: xml操作类
 * 其它说明: 
 * 作    者:peng na
 * 完成日期:2016年9月2日
 * 修改记录:
 * ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Xml.Linq;


namespace GDICommon
{
    public class XMLOperater
    {
        
        private XmlDocument objXmlDoc = null;

        public XMLOperater()
        { 
            objXmlDoc= new XmlDocument();
        }

        #region 动态方法
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：加载xml文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool LoadXml(string filePath)
        {
            try
            {
                if (File.Exists(filePath) == false)
                {
                    return false;
                }
                objXmlDoc.Load(filePath);
                return true;
            }
            catch  
            {
                return false;
            }
           
        }
        public bool LoadXmlContent(string xml)
        {
            try
            {
                if (xml == "")
                {
                    return false;
                }
                objXmlDoc.LoadXml(xml);
                return true;
            }
            catch  
            {
                return false;
            }
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：将根节点添加到XMLDocu中
        /// </summary>
        /// <param name="rootNode"></param>
        public void AppendNodeToDoc(XmlNode rootNode)
        {
            this.objXmlDoc.AppendChild(rootNode);
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：获取节点的属性
        /// </summary>
        /// <param name="node">节点名称</param>
        /// <param name="xmlArrtiName">属性名层</param>
        /// <returns>节点属性</returns>
        public XmlAttribute GetProperCollection(XmlNode node, string xmlArrtiName)
        {
            XmlAttribute xmlArrtiTemp = null;
            foreach (XmlAttribute xmlAttri in node.Attributes)
            {
                if (xmlAttri.Name == xmlArrtiName)
                {
                    xmlArrtiTemp = xmlAttri;
                    break;
                }
            }
            return xmlArrtiTemp;
        }

        /// <summary>
        ///作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：通过节点名称及属性名称获取节点
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="properName">属性名称</param>
        /// <returns>节点node</returns>
        public XmlNode GetNodeByName(string nodeName,string properName)
        {
            XmlNodeList nodeList =  objXmlDoc.GetElementsByTagName(nodeName);
            foreach(XmlNode node in nodeList)
            {
                foreach(XmlAttribute attri in node.Attributes)
                {
                    if ( attri.Value == properName)
                    {
                        return node;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：根据节点全路径获取节点的view
        /// </summary>
        /// <param name="XmlPathNode">节点路径</param>
        /// <returns>数据view</returns>
        public DataView GetData(string XmlPathNode)
        {
            //查找数据。返回一个DataView
            DataSet ds = new DataSet();
            StringReader read = new StringReader(objXmlDoc.SelectSingleNode(XmlPathNode).OuterXml);
            ds.ReadXml(read);
            return ds.Tables[0].DefaultView;
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：通过节点名称名称获取节点
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <returns>节点</returns>
        public XmlNode GetNodeByName(string nodeName)
        {
            XmlNodeList xmlList =  objXmlDoc.GetElementsByTagName(nodeName);
            if (xmlList != null && xmlList.Count > 0)
            {
                return xmlList[0];

            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：设置接单值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="Content">节点值</param>
        public void SetNodeValue(string nodeName, string Content)
        {
            //更新节点内容。
           XmlNode node= GetNodeByName(nodeName);
           if (node != null)
           {
               node.InnerText = Content;
           }
         
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：删除节点
        /// </summary>
        /// <param name="Node">节点名称</param>
        public void Delete(string Node)
        {
            //删除一个节点。
            string mainNode = Node.Substring(0, Node.LastIndexOf("/"));
            objXmlDoc.SelectSingleNode(mainNode).RemoveChild(objXmlDoc.SelectSingleNode(Node));
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：向节点中插入子节点
        /// </summary>
        /// <param name="MainNode">父节点</param>
        /// <param name="ChildNode">子节点</param>
        /// <param name="Element">子节点名称</param>
        /// <param name="Content">子节点内容</param>
        public void InsertNode(string MainNode, string ChildNode, string Element, string Content)
        {
            //插入一节点和此节点的一子节点。
            XmlNode objRootNode = objXmlDoc.SelectSingleNode(MainNode);
            XmlElement objChildNode = objXmlDoc.CreateElement(ChildNode);
            objRootNode.AppendChild(objChildNode);
            XmlElement objElement = objXmlDoc.CreateElement(Element);
            objElement.InnerText = Content;
            objChildNode.AppendChild(objElement);
        }

        public void InsertElement(string MainNode, string Element, string Attrib, string AttribContent, string Content)
        {
            //插入一个节点，带一属性。
            XmlNode objNode = objXmlDoc.SelectSingleNode(MainNode);
            XmlElement objElement = objXmlDoc.CreateElement(Element);
            objElement.SetAttribute(Attrib, AttribContent);
            objElement.InnerText = Content;
            objNode.AppendChild(objElement);
        }

        public void InsertElement(string MainNode, string Element, string Content)
        {
            //插入一个节点，不带属性。
            XmlNode objNode = objXmlDoc.SelectSingleNode(MainNode);
            XmlElement objElement = objXmlDoc.CreateElement(Element);
            objElement.InnerText = Content;
            objNode.AppendChild(objElement);
        }

        /// <summary>
        /// 作者：np
        /// 时间：2013.6.4
        /// 内容：创建xml文件
        /// </summary>
        public void CreateXmlFile(string xmlFilePath)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmldec = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmldoc.AppendChild(xmldec);
            xmldoc.Save(xmlFilePath);
        }

        /// <summary>
        /// 作者：np
        /// 时间：2013.6.4
        /// 内容：读取xml文件
        /// </summary>
        /// <returns></returns>
        public DataSet ReadFile(string filePath)
        {
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
       /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：获取节点通过节点名称
       /// </summary>
       /// <param name="nodeName">节点名称</param>
       /// <returns>节点列表</returns>
        public  XmlNodeList GetNodesByName(string nodeName)
        {
            return objXmlDoc.GetElementsByTagName(nodeName);
        }
        /// <summary>
        /// 作  者：np
        /// 时  间：2016年9月28日
        /// 内  容：保存xml
        /// </summary>
        /// <param name="filePath">路径</param>
        public void Save(string filePath)
        {
            try
            {
                objXmlDoc.Save(filePath);
            }
            catch 
            {
         
            }

        }

      

        /// <summary>
        /// ytj
        /// 2014.12.5
        /// 通过节点值找到指定节点
        /// </summary>
        /// <param name="Element"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public DataSet GetDataByElement(string Element, string Content)
        {
            DataSet ds = new DataSet();

            XmlElement rootnode = objXmlDoc.DocumentElement;
            XElement xelement = XElement.Parse(objXmlDoc.InnerXml);

            var finder = xelement.Elements("Inst").First((t) => t.Element(Element).Value == Content);
            XmlReader xmlReader = finder.CreateReader();

            ds.ReadXml(xmlReader);

            return ds;
        }

        #endregion
        #region 静态方法
        /// <summary>
        /// 作者：np
        /// 时间：2015年3月19日
        /// 内容：创建带有属性的节点默认属性名称为type
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public  XmlNode CreateNodeWithAttribute(string nodeName, string nodeValue,string attributeName, string attributeValue)
        {
            if (nodeValue == null)
            {
                nodeValue = "";
            }
            if (nodeName == null)
            {
                nodeName = "";
            }
            XmlElement objElement = objXmlDoc.CreateElement(nodeName);
            objElement.InnerText = nodeValue;
            if (false == string.IsNullOrEmpty(attributeValue))
            {
                objElement.SetAttribute(attributeName, attributeValue);
            }
            return objElement;
        }
      

        #endregion
    }

    
    
}
