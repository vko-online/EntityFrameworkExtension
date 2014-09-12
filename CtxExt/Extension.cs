using CtxExt.Addons;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CtxExt
{
    public static class Extension
    {
        /// <summary>
        /// Добавление/Сохранение сущности
        /// </summary>
        /// <typeparam name="CONTEXT">Используемый контекст. Причемание: контекст должен быть инициализирован</typeparam>
        /// <typeparam name="T">Тип Т существующий в контексте CONTEXT</typeparam>
        /// <param name="ctx"></param>
        /// <param name="item">Сущность</param>
        /// <param name="primaryKey">Первичный ключ, используется при добавлении записи</param>
        /// <returns></returns>
        public static T SAVE<CONTEXT, T>(this DbContext ctx, T item, string primaryKey = null)
            where T : class
            where CONTEXT : DbContext
        {
            if (item == null)
                throw new NullReferenceException("Ссылка на Т не указывает на экземпляр");
            if (ctx == null)
                throw new NullReferenceException("Ссылка на CONTEXT не указывает на экземпляр");
            PropertyInfo prop = null;
            if (primaryKey != null)
                prop = item.GetType().GetProperty(primaryKey);
            if (prop == null)
                prop = item.GetType().GetProperty("Id");
            if (prop == null)
                prop = item.GetType().GetProperty("ID");
            if (prop == null)
                prop = item.GetType().GetProperty("id");
            if (prop == null)
                throw new NullReferenceException("Не существует первичный ключ. Попытки с ID, Id, id прошли безуспешно");

            var id = prop.GetValue(item);
            if (id is long || id is int)
                if (Convert.ToInt64(id) == 0)
                {
                    ctx.Set<T>().Add(item);
                    ctx.Entry(item).State = EntityState.Added;
                    ctx.SaveChanges();
                    return item;
                }
            if (id is string || id is Guid)
                if (Convert.ToString(id) == string.Empty)
                {
                    ctx.Set<T>().Add(item);
                    ctx.Entry(item).State = EntityState.Added;
                    ctx.SaveChanges();
                    return item;
                }
            var origin = ctx.Set<T>().Find(id);
            origin = Fill<T>(item, origin);
            ctx.Entry(origin).State = EntityState.Modified;
            ctx.SaveChanges();
            return item;
        }

        public static T GET<CONTEXT, T>(this DbContext ctx, long id)
            where T : class
            where CONTEXT : DbContext
        {
            if (ctx == null)
                throw new NullReferenceException("Ссылка на CONTEXT не указывает на экземпляр");
            return ctx.Set<T>().Find(id);
        }

        public static IQueryable<T> ALL<CONTEXT, T>(this DbContext ctx)
            where T : class
            where CONTEXT : DbContext
        {
            if (ctx == null)
                throw new NullReferenceException("Ссылка на CONTEXT не указывает на экземпляр");
            return ctx.Set<T>().AsQueryable();

        }

        internal static T Fill<T>(object source, T target = null, string[] restricts = null) where T : class
        {
            if (target == null)
            {
                throw new NullReferenceException("Ссылка на T не указывает на экземпляр");
            }

            var properties = source.GetType().GetProperties();

            foreach (var info in properties)
            {
                if (restricts != null)
                {
                    if (!restricts.Contains(info.Name.ToLower(), new StringComparator()))
                        TrySetValue(target, info, info.GetValue(source));
                }
                else
                {
                    TrySetValue(target, info, info.GetValue(source));
                }
            }

            return target;
        }

        internal static void TrySetValue(object target, PropertyInfo info, object value)
        {
            var targetProperty = target.GetType().GetProperty(info.Name);

            if (targetProperty != null)
            {
                targetProperty.SetValue(target, value);
            }
        }
    }
}
