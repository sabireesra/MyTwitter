using MyTwitter.Models.Abstracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MyTwitter.Models.Repository
{
    public abstract class AbstractBaseRepository<T> : IDisposable
      where T : class, IBaseModel
    {
        internal DatabaseContext context = null;

        public DbSet<T> Entity { get { return context.Set<T>(); } }

        public AbstractBaseRepository()
        {
            context = new DatabaseContext();
        }

        public virtual bool Add(T entity)
        {
            entity.CreateDate = DateTime.Now;
            Entity.Add(entity);

            return context.SaveChanges() > 0;
        }

        public virtual T Find(int Id)
        {
            return Entity.FirstOrDefault(x => x.Id == Id);
        }

        public virtual bool Delete(T entity)
        {
            if (entity != null && entity.Id != default(int))
            {
                var record = Find(entity.Id);
                if (record == null)
                {
                    throw new NullReferenceException(nameof(entity.Id));
                }
                record.IsDeleted = true;
                record.DeleteDate = DateTime.Now;

                return context.SaveChanges() > 0;

            }

            throw new ArgumentOutOfRangeException(nameof(entity.Id));
        }

        public virtual bool Update(T entity)
        {
            if (entity != null && entity.Id != default(int))
            {
                var record = Find(entity.Id);
                if (record == null)
                {
                    throw new NullReferenceException(nameof(entity.Id));
                }

                record = entity;
                record.UpdateDate = DateTime.Now;

                return context.SaveChanges() > 0;
            }

            throw new ArgumentOutOfRangeException(nameof(entity.Id));
        }

        public virtual IQueryable<T> ListAll()
        {
            return Entity.Where(x => !x.IsDeleted);
        }

        public virtual int Count()
        {
            return ListAll().Count();
        }

        public virtual IQueryable<T> ListPaged(Expression<Func<T, bool>> expression, int pageNumber, int pageSize = 10)
        {
            var totalRecordCount = Count();
            var currentPage = pageNumber == 1 ? 0 : pageNumber - 1;

            return ListAll()
                .OrderByDescending(x => x.CreateDate)
                .Where(expression)
                .Skip(pageSize * currentPage)
                .Take(pageSize);
        }

        public IQueryable<K> Query<K>() where K : class
        {
            return context.Set<K>();
        }
        public void UnFollow(int id)
        {
            UserFriends model = context.UserFriends.Where(k => k.Id == id).FirstOrDefault();
            context.UserFriends.Remove(model);
            context.SaveChanges();

        }

        readonly string _connString = ConfigurationManager.ConnectionStrings["TwitterConnectionString"].ConnectionString;


        public IEnumerable<Tweet> GetAllMessages()
        {
            var messages = new List<Tweet>();

            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();


                using (var command = new SqlCommand(@"SELECT [Id], [UserId], [Body], [ParentId],[CreateDate],[UserName] FROM [dbo].[Tweets]", connection))
                {

                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(Dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();



                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        messages.Add(item: new Tweet
                        {
                            Id = (int)reader["Id"],
                            UserId = (int)reader["UserId"],
                            Body = reader["Body"] != DBNull.Value ? (string)reader["Body"] : "",
                            ParentId = (int)reader["ParentId"],
                            CreateDate = Convert.ToDateTime(reader["CreateDate"]),
                            UserName = reader["UserName"] != DBNull.Value ? (string)reader["UserName"] : ""
                        });
                    }
                }

            }
            return messages;


        }
        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MessagesHub.SendMessages();
            }
        }        
        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }
    }



    public class BaseRepository<T> : AbstractBaseRepository<T>
        where T : class, IBaseModel
    {

    }
}