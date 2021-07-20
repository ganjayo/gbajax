using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace gbajax.Models
{
    public class Guestbooks
    {
        [DisplayName("編號：")]
        public int ID { get; set; }
        [DisplayName("名字：")]
        [Required(ErrorMessage="請輸入名字")]
        [StringLength(20, ErrorMessage ="名字不可以超過20字元")]
        public string ACCOUNT { get; set; }
        [DisplayName("留言內容：")]
        [Required(ErrorMessage = "請輸入留言內容")]
        [StringLength(20, ErrorMessage = "留言內容不可以超過100字元")]
        public string CONTENT { get; set; }
        [DisplayName("新增時間：")]
        public DateTime CREATETIME { get; set; }
        [DisplayName("回復內容：")]
        [Required(ErrorMessage = "請輸入回復內容")]
        [StringLength(20, ErrorMessage = "回復內容不可以超過100字元")]
        public string REPLY { get; set; }
        [DisplayName("回覆時間：")]
        public DateTime? REPLYTIME { get; set; }

        public Members Member { get; set; } = new Members();
    }
}