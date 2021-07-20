using gbajax.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gbajax.Service
{
    public class GuestbooksDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ganjayo"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            if(!string.IsNullOrWhiteSpace(Search))
            {
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
            }

            else
            {

                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
            }


            return DataList;
        }

        public void InsertGuestbooks(Guestbooks newData)
        {
            string sql = $@"INSERT INTO messageboard(ACCOUNT,CONTENT,CREATETIME) VALUES('{newData.ACCOUNT}','{newData.CONTENT}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public Guestbooks GetDataByID(int ID)


        {
            Guestbooks Data = new Guestbooks();
            string sql = $@"SELECT * FROM messageboard m inner join Members d on m.Account = d.Account WHERE ID ={ID};";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.ID = Convert.ToInt32(dr["ID"]);
                Data.ACCOUNT = dr["Name"].ToString();
                Data.CONTENT = dr["CONTENT"].ToString();
                Data.CREATETIME = Convert.ToDateTime(dr["CREATETIME"]);

                if (!string.IsNullOrWhiteSpace(dr["REPLY"].ToString()))
                {
                    Data.REPLY = dr["REPLY"].ToString();
                    Data.REPLYTIME = Convert.ToDateTime(dr["REPLYTIME"]);
                }

                Data.Member.Name = dr["Name"].ToString(); 


            }

            catch(Exception e )
            {
                Data = null;
            }

            finally
            {
                conn.Close();
            }

            return Data;
        }
        public void UpdateGuestbooks(Guestbooks UpdateData)


        {
            string sql = $@"UPDATE messageboard SET NAME = '{UpdateData.ACCOUNT}', CONTENT ='{UpdateData.CONTENT}' WHERE ID = {UpdateData.ID};";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }

            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }

            finally
            {
                conn.Close();
            }

        }

        public void ReplyGuestbooks(Guestbooks ReplyData)


        {
            string sql = $@"UPDATE messageboard SET REPLY = '{ReplyData.REPLY}', REPLYTIME ='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE ID = {ReplyData.ID};";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }

            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }

            finally
            {
                conn.Close();
            }

        }

        public bool CheckUpdate(int ID)
        {
            Guestbooks Data = GetDataByID(ID);
            return (Data != null && Data.REPLYTIME == null);
        }
        public void DeletGuestbooks(int ID)
        {
            string sql = $@"DELETE FROM messageboard WHERE ID={ID};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }

            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }

            finally
            {
                conn.Close();
            }

        }

        public void SetMaxPaging(ForPaging Paging)
        {
            int Row = 0;
            string sql = $@"SELECT * FROM messageboard;";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    Row++;
                }
            }

            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row)/ Paging.ItemNum));
            Paging.SetRightPage();
        }

        public void SetMaxPaging(ForPaging Paging, string Search)
        {
            int Row = 0;
            string sql = $@"SELECT *FROM messageboard WHERE 
CONTENT LIKE '%{Search}%' OR REPLY LIKE '%{Search}%';";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Row++;
                }
            }

            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            Paging.SetRightPage();
        }

        public List<Guestbooks> GetAllDataList(ForPaging paging)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            string sql = $@" SELECT m.*, d.Name, d.IsAdmin FROM (SELECT row_number() OVER(order by ID) AS sort, * FROM messageboard) m inner join Members d on m.Account = d.Account where m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Guestbooks Data = new Guestbooks();
                    Data.ID = Convert.ToInt32(dr["ID"]);
                    Data.ACCOUNT = dr["Name"].ToString();
                    Data.CONTENT = dr["CONTENT"].ToString();
                    Data.CREATETIME = Convert.ToDateTime(dr["CREATETIME"]);


                    if (!dr["REPLYTIME"].Equals(DBNull.Value))
                    {
                        Data.REPLY = dr["REPLY"].ToString();
                        Data.REPLYTIME = Convert.ToDateTime(dr["REPLYTIME"]);
                    }
                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return DataList;
        }
        public List<Guestbooks> GetAllDataList(ForPaging paging, string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            string sql = $@" SELECT  m.*, d.Name, d.IsAdmin FROM (SELECT row_number() OVER(order by ID) AS sort, * FROM messageboard WHERE NAME LIKE '%{Search}%' OR CONTENT LIKE '%{Search}%' OR REPLY LIKE '%{Search}%') m inner join Members d on m.Account = d.Account where m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Guestbooks Data = new Guestbooks();
                    Data.ID = Convert.ToInt32(dr["ID"]);
                    Data.ACCOUNT = dr["Name"].ToString();
                    Data.CONTENT = dr["CONTENT"].ToString();
                    Data.CREATETIME = Convert.ToDateTime(dr["CREATETIME"]);


                    if (!dr["REPLYTIME"].Equals(DBNull.Value))
                    {
                        Data.REPLY = dr["REPLY"].ToString();
                        Data.REPLYTIME = Convert.ToDateTime(dr["REPLYTIME"]);
                    }

                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return DataList;
        }

    }

}