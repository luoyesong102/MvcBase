using System.Collections.Generic;

namespace Esmart.Permission.Application.Constants
{
    static class BuiltinRoles
    {
     

        public const string TeachingAdmin = "业务A管理员";
        public const string EducationAdmin = "业务B管理员";
        public const string StudyConsultantAdmin = "业务C管理员";
        public const string ScheduleConsultantAdmin = "业务D管理员";


        public readonly static ReadonlyHashSet<string> All;
        public readonly static ReadonlyHashSet<string> Admins;

        static BuiltinRoles()
        {
            var all = new[]
            {
               

                TeachingAdmin,
                EducationAdmin,
                StudyConsultantAdmin,
                ScheduleConsultantAdmin
            };

            All = new ReadonlyHashSet<string>(all);

            var admins = new[]
            {
                TeachingAdmin,
                EducationAdmin,
                StudyConsultantAdmin,
                ScheduleConsultantAdmin
            };

            Admins = new ReadonlyHashSet<string>(admins);
        }
    }

    class ReadonlyHashSet<T> : IEnumerable<T>
    {
        readonly HashSet<T> _set;

        public ReadonlyHashSet(IEnumerable<T> collection)
        {
            _set = new HashSet<T>(collection);
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _set.GetEnumerator();
        }
    }
}
