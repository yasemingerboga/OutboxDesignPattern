using ApplicationNotification.Repository;
using DomainNotification;
using PersistanceNotification.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceNotification.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(NotificationDbContext context) : base(context)
        {

        }
    }
}
