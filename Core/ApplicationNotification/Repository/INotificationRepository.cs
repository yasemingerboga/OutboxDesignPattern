using Application.Repositories;
using DomainNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationNotification.Repository
{
    public interface INotificationRepository : IRepository<Notification>
    {
    }
}