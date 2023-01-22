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
    public class NotificationOutboxRepository : Repository<NotificationOutbox>, INotificationOutboxRepository
    {
        public NotificationOutboxRepository(NotificationDbContext context) : base(context)
        {

        }
    }
}
