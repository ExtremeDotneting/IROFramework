using System;
using System.Linq;
using Libs.Metadata;

namespace Libs
{
    public static class EnumExt
    {
        /// <summary>
        /// Enum must be reflectable.
        /// <para></para>
        /// Support <see cref="EnumItemTextAttribute"/>.
        /// </summary>
        public static TEnum FromInteger<TEnum>(int enumNum)
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            var enums = enumType.GetEnumValues();
            foreach (var item in enums)
            {
                var itemAsEnum = item.Cast<TEnum>();
                var currentNum=itemAsEnum
                    .ValueOf()
                    .Cast<int>();
                if (currentNum.Equals(enumNum))
                    return itemAsEnum;
            }

            throw new Exception($"Can't find enum by int '{enumNum}'.");
        }

        /// <summary>
        /// Enum must be reflectable.
        /// <para></para>
        /// Support <see cref="EnumItemTextAttribute"/>.
        /// </summary>
        public static TEnum FromString<TEnum>(string enumStr)
            where TEnum : struct, Enum
        {

            if (Enum.TryParse(enumStr, out TEnum defaultParseEnum))
            {
                return defaultParseEnum;
            }

            var enumType = typeof(TEnum);
            var memberInfos = enumType.GetMembers();
            foreach (var member in memberInfos)
            {
                var valueAttributes = member
                    .GetCustomAttributes(typeof(EnumItemTextAttribute), false);
                if (valueAttributes.Any())
                {
                    var text = ((EnumItemTextAttribute)valueAttributes[0]).Text;

                    if (text != null && text.Equals(enumStr))
                    {
                        var parsedEnum = Enum.Parse(enumType, member.Name);
                        return (TEnum)parsedEnum;
                    }
                }
            }

            throw new Exception($"Can't convert '{enumStr}' to '{typeof(TEnum)}'.");
        }

        /// <summary>
        /// Enum must be reflectable.
        /// <para></para>
        /// Support <see cref="EnumItemTextAttribute"/>.
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static string AsString(this Enum enumObj)
        {
            //This works for enums serialized as numbers.
            var basicEnumStr = GetNameBasic(enumObj);
            var enumType = enumObj.GetType();
            var fromAttribute = GetNameFromAttribute(enumType, basicEnumStr);
            return fromAttribute ?? basicEnumStr;
        }

        /// <summary>
        /// Enum must be reflectable.
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static int AsInteger(this Enum enumObj)
        {
            try
            {
                //If enum is integer.
                var num = enumObj
                    .ValueOf()
                    .Cast<int>();
                return num;
            }
            catch
            {
                //If enums is strings (only bridge.net).
                var enums = enumObj.GetType().GetEnumValues();
                var index = 0;
                foreach (var item in enums)
                {
                    if (item.Equals(enumObj))
                        return index;
                    index++;
                }
                throw new ArgumentOutOfRangeException(nameof(enumObj));
            }


        }

        static string GetNameBasic(Enum enumObj)
        {
            //This works for enums serialized as numbers.
            var str = enumObj.ToString();
            if (char.IsNumber(str[0]))
            {
                return enumObj.GetType().GetEnumName(enumObj);
            }
            else
            {
                return str;
            }
        }

        static string GetNameFromAttribute(Type enumType, string basicEnumStr)
        {
            try
            {

                var memberInfos = enumType.GetMember(basicEnumStr);
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                var valueAttributes =
                    enumValueMemberInfo.GetCustomAttributes(typeof(EnumItemTextAttribute), false);
                var text = ((EnumItemTextAttribute)valueAttributes[0]).Text;
                return text;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}