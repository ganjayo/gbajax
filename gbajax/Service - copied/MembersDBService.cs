using gbajax.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace gbajax.Service
{

    public class MembersDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ganjayo"].ConnectionString;
        private readonly SqlConnection conn = new SqlConnection(cnstr);



        public void Register (Members newMember)
        {
            newMember.Password = HashPassword(newMember.Password);
            string sql = $@" INSERT INTO members (Account, Password, Name, Email, AuthCode, IsAdmin) VALUES ('{newMember.Account}','{newMember.Password}','{newMember.Name}','{newMember.Email}','{newMember.AuthCode}','0')";


            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }

            catch (Exception e)
            {
                throw new Exception (e.Message.ToString());
            }

            finally
            {
                conn.Close();
            }
        }

        public string HashPassword(string Password)
        {
            string saltkey = "dfddsdl53d1f2";
            string saltAndPassword = String.Concat(Password, saltkey);
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);
            string Hashresult = Convert.ToBase64String(HashDate);
            return Hashresult;

        }

        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            string sql = $@" select * from members where Account = '{Account}'";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);

            }

            catch(Exception e)
            {
                Data = null;
            }

            finally
            {
                conn.Close();
            }

            return Data;
        }


        public bool AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);
            bool result = (Data == null);
            return result;
        }

        public string EmailValidate(string Account, string AuthCode)
        {
            Members ValidateMember = GetDataByAccount(Account);
            string ValidateStr = string.Empty;
            if (ValidateMember != null)
            {
                if (ValidateMember.AuthCode == AuthCode)
                {
                    string sql = $@" update members set AuthCode ='{string.Empty}' where Account = '{Account}'";

                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                    }

                    catch (Exception e)
                    {
                        throw new Exception (e.Message.ToString());
                    }

                    finally
                    {
                        conn.Close();
                    }
                    ValidateStr = "帳號信箱驗證成功，現在可以登入了";
                   
                }

                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認再註冊";
                }

            }

            else
            {
                   ValidateStr = "傳送資料錯誤，請重新確認再註冊";
            }

            return ValidateStr;
        }

        public string LoginCheck(string Account, string Password)
        {
            Members LoginMenber = GetDataByAccount(Account);

            if (LoginMenber != null)
            {
                if (String.IsNullOrWhiteSpace(LoginMenber.AuthCode))
                {
                    if(PasswordCheck(LoginMenber, Password))
                    {
                        return "";
                    }

                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }

                else
                {
                    return "此帳號密碼尚未通過Email驗證，請去收信";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }
        }

        public bool PasswordCheck(Members CheckMember, string Password)
        {
            bool result = CheckMember.Password.Equals(HashPassword(Password));
            return result;
        }

        public string GetRole(string Account)
        {

            string Role = "User";
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember.IsAdmin)
            {
                Role += ", Admin";
            }

            return Role;

        }

        public string ChangePassword(string Account, string Password, string newPassword)
        {
            Members LoginMember = GetDataByAccount(Account);
            if (PasswordCheck(LoginMember, Password))
            {
                LoginMember.Password = HashPassword(newPassword);
                string sql = $@" update Members set Password = '{LoginMember.Password}' where Account ='{Account}'";
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

                return "密碼修改成功";

            }

            else
            {
                return "舊密碼輸入錯誤";
            }
        }
       

    }
}