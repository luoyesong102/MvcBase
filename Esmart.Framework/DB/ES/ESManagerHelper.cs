
using PlainElastic.Net;
using PlainElastic.Net.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Esmart.Framework.DB
{
 
    public class ESManagerHelper
    {
        
       
        //protected JsonNetSerializer _serializer = new JsonNetSerializer();

        protected string _esIndex;
        protected string _esType;
        private ElasticConnection _connection = null;//SuperWebSocket中的WebSocketServer对象
        public ElasticConnection connection
        {
            get { return _connection; }
        }
        public ESManagerHelper()
        {
        }

        public void InitConnectionNew(string connStr)
        {
            try
            {
                string ip = connStr.Split('/')[0];
                _connection = new ElasticConnection(ip.Split(':')[0], Convert.ToInt32(ip.Split(':')[1]));
                _esIndex = connStr.Split('/')[1];
                _esType = connStr.Split('/')[2];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}初始化错误，可能[{0}]ES数据库挂了， 快找运维解决！", connStr), ex);
            }
        }

        public T AutoTry<T>(Func<T, T> ac)
        {
            dynamic result = (T) Activator.CreateInstance(typeof (T));
            try
            {
                result = ac.Invoke(result);
            }
            catch (Exception ex)
            {
                result.ServiceCode = 1;
                result.ErrorResult = ex.Message.ToString();
            }
            return result;
        }


        public string GetGUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 获取esCommand
        /// </summary>
        /// <param name="srcLanguageCode">当类型为search、deletebyquery、count时可以为空值</param>
        /// <param name="type">类型：search、update、index、deletebyquery、delete、count、bulk</param>
        /// <param name="id">用于更新删除等 如果类型为update、delete、index则id不可为空</param>
        /// <returns></returns>
        public string GetCommand(int type, string id)
        {
            string strCommand = string.Empty;
            string strDb = _esIndex;
            string strTable = _esType;
            switch (type)
            {
                case 0:
                    strCommand = Commands.Search(strDb, strTable);
                    break;
                case 1:
                    if (!string.IsNullOrEmpty(id))
                        strCommand = Commands.Index(strDb, strTable, id);
                    break;
                case 2:
                    if (!string.IsNullOrEmpty(id))
                        strCommand = Commands.UpdateSettings(strDb);
                    
                    break;
                case 3:
                    strCommand = Commands.DeleteByQuery(strDb, strTable);
                    break;
                case 4:
                    if (!string.IsNullOrEmpty(id))
                        strCommand = Commands.Delete(strDb, strTable, id);
                    break;
                case 5:
                    strCommand = Commands.Count(strDb, strTable);
                    break;
                case 6:
                    strCommand = Commands.Bulk(strDb, strTable);
                    break;
                default:
                    strCommand = Commands.Search(strDb, strTable);
                    break;
            }
            return strCommand;
        }


        /// <summary>
        /// 排序的方法
        /// </summary>
        /// <param name="orderby">排序</param>
        /// <param name="tableName">所属对象</param>
        /// <returns></returns>
        public string Orderby(Dictionary<string, int> orderby, string tableName = "")
        {
            string orderStr = string.Empty;
            if (orderby != null && orderby.Count > 0)
            {
                foreach (var item in orderby)
                {
                    if (item.Value < 0)
                        orderStr += string.IsNullOrEmpty(orderStr)
                            ? "{\"" + (!string.IsNullOrEmpty(tableName) ? tableName + "." : "") + item.Key +
                              "\": {\"order\": \"desc\"}}"
                            : "," + "{\"" + (!string.IsNullOrEmpty(tableName) ? tableName + "." : "") + item.Key +
                              "\": {\"order\": \"desc\"}}";
                    else
                        orderStr += string.IsNullOrEmpty(orderStr)
                            ? "{\"" + (!string.IsNullOrEmpty(tableName) ? tableName + "." : "") + item.Key +
                              "\": {\"order\": \"asc\"}}"
                            : "," + "{\"" + (!string.IsNullOrEmpty(tableName) ? tableName + "." : "") + item.Key +
                              "\": {\"order\": \"asc\"}}";
                }
            }
            else
            {
                orderStr = " ";
            }
            return orderStr;
        }

        public void RefreshIndex()
        {
            try
            {
                string searchCommand = Commands.Refresh(_esIndex);
                _connection.Post(searchCommand);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ES数据库挂了， 快找运维解决！{0}", ex.Message));
            }
        }

        //后期优化，可将ES查询进行序列化 TODO.....
    }
}
