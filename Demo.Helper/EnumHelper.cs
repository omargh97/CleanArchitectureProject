using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Demo.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            string desc = value.ToString();

            FieldInfo info = value.GetType().GetField(desc);
            var attrs = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                desc = attrs[0].Description;
            }

            return desc;
        }

        public static EnumSortMode GetSortMode(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Target type must be an enum.");
            }

            EnumSortMode sortMode = EnumSortMode.ByName;

            var info = enumType;
            var attrs = (EnumSortModeAttribute[])info.GetCustomAttributes(typeof(EnumSortModeAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                sortMode = attrs[0].SortMode;
            }

            return sortMode;
        }

        public static T Parse<T>(string value) where T : struct
        {
            return Parse<T>(value, false);
        }

        public static T Parse<T>(string value, bool ignoreCase) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enum type.");
            }

            var result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            return result;
        }

        public static T ToEnum<T>(this string value) where T : struct
        {
            return Parse<T>(value);
        }

        public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            return Parse<T>(value, ignoreCase);
        }
    }

    public enum EnumSortMode
    {
        ByName,
        ByValue
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumSortModeAttribute : Attribute
    {
        public EnumSortModeAttribute(EnumSortMode sortMode)
        {
            SortMode = sortMode;
        }

        public EnumSortMode SortMode { get; private set; }
    }
}

