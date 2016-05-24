﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Wodsoft.ComBoost.Data.Entity.Metadata;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Data
{
    public class EntityDomainService<T> : DomainService
        where T : class, new()
    {
        public IEntityMetadata Metadata { get; private set; }

        public EntityDomainService()
        {
            Metadata = EntityAnalyzer.GetMetadata<T>();
        }

        public virtual Task Create([FromService] IDatabaseContext database, [FromService] IAuthenticationProvider authenticationProvider)
        {
            if (!Metadata.AllowAnonymous)
            {
                var auth = authenticationProvider.GetAuthentication();
                if (auth == null)
                    throw new NotSupportedException("不能从当前权限提供器获取权限。");
                if (Metadata.AuthenticationRequiredMode == System.ComponentModel.DataAnnotations.AuthenticationRequiredMode.All)
                    if (Metadata.AddRoles.Any(t => !auth.IsInRole(t)))
                        throw new UnauthorizedAccessException("创建权限不足。");
            }
            var context = database.GetContext<T>();
            EntityEditModel<T> model = new EntityEditModel<T>(context.Create());
            ExecutionContext.DomainContext.Result = model;
            return Task.FromResult(0);
        }

        public virtual async Task Update([FromService] IDatabaseContext database, [FromService] IValueProvider valueProvider)
        {
            var context = database.GetContext<T>();
            object index = valueProvider.GetRequiredValue(Metadata.KeyProperty.ClrName);
            if (index.GetType() != Metadata.KeyProperty.ClrType)
            {
                var converter = TypeDescriptor.GetConverter(Metadata.KeyProperty.ClrType);
                index = converter.ConvertFrom(index);
            }
            var entity = await context.GetAsync(index);
            if (entity == null)
                return;
            await UpdateCore(valueProvider, entity);
            //ExecutionContext.DomainContext.Result = await database.SaveAsync();
        }

        protected virtual Task UpdateCore(IValueProvider valueProvider, T entity)
        {
            //Metadata.
            return null;
        }

        protected virtual Task UpdateProperty(IValueProvider valueProvider, T entity)
        {
            return null;

        }

        public virtual Task Remove(IDomainContext serviceContext)
        {
            return null;

        }

        public virtual Task Detail(IDomainContext serviceContext)
        {
            return null;
        }
    }
}
