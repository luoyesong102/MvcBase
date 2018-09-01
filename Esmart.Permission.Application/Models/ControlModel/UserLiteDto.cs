using System;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class UserLiteDto : IEquatable<UserLiteDto>
    {
        public int UserID { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        public string Ename { get; set; }

        /// <summary>
        /// 员工号
        /// </summary>
        public string WorkNo { get; set; }

        public int? Sex { get; set; }

        public bool Equals(UserLiteDto other)
        {
            return other != null && other.UserID == UserID;
        }

        public override bool Equals(object obj)
        {
            var other = obj as UserLiteDto;
            return other != null && other.UserID == UserID;
        }

        public override int GetHashCode()
        {
            return UserID;
        }
    }

    public class UpdateUserDto : UserLiteDto
    {
        public string Mobile { get; set; }
        
        public string qq { get; set; }

        public string HomeAddr { get; set; }

        public int Isleave { get; set; }
    }
}
