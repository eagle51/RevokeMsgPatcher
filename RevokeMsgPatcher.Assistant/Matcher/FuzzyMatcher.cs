﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RevokeMsgPatcher.Assistant.Matcher
{
    /// <summary>
    /// 对16进制数据进行模糊查找
    /// 通配符的方式
    /// </summary>
    public class FuzzyMatcher
    {
        public const byte wildcard = 0x3F; // 通配符

        /// <summary>
        /// 模糊匹配所有
        /// </summary>
        /// <param name="content">被查找对象</param>
        /// <param name="pattern">查找串</param>
        /// <returns></returns>
        public static int[] MatchAll(byte[] content, byte[] pattern)
        {
            byte[] head = GetHead(pattern);
            int[] indexs = BoyerMooreMatcher.MatchAll(content, head);
            // 头串和查找串相同则直接返回，不同则继续判断是否符合查询串
            if (head.Length == pattern.Length)
            {
                return indexs;
            }
            else
            {
                List<int> res = new List<int>();
                foreach (int index in indexs)
                {
                    if (IsEqual(content, index, pattern))
                    {
                        res.Add(index);
                    }
                }
                if (res.Count > 0)
                {
                    return res.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取头串
        /// </summary>
        /// <param name="whole">完整查找串</param>
        /// <returns></returns>
        private static byte[] GetHead(byte[] whole)
        {
            int len = whole.Length;
            for (int i = 0; i < whole.Length; i++)
            {
                if (whole[i] == wildcard)
                {
                    len = i;
                    break;
                }
            }
            if (len == 0)
            {
                throw new Exception("不正确的通配符位置!");
            }
            return whole.Take(len).ToArray();
        }

        /// <summary>
        /// 确认整个查找串是否匹配
        /// </summary>
        /// <param name="content">被查找对象</param>
        /// <param name="start">头串匹配位置</param>
        /// <param name="whole">完整查找串</param>
        /// <returns></returns>
        private static bool IsEqual(byte[] content, int start, byte[] whole)
        {
            int i = 0;
            for (i = 0; i < whole.Length; i++)
            {
                if (whole[i] == wildcard)
                {
                    continue;
                }
                if (content[start + i] != whole[i])
                {
                    break;
                }
            }
            if (i == whole.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
