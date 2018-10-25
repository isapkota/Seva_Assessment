using FluentNHibernate.Mapping;
using Seva.Assessment.FluentNHibernetImpl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seva.Assessment.DataService.User
{
    public class UserData: Entity
    {
        public virtual string EmailAddress { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

        public virtual IList<UserInterest> Interest { get; set; }
    }

    public class UserInterest : Entity
    {
        public virtual int UserID { get; set; }
        public virtual string Interest { get; set; }
    }


    public class UserDataMap :  ClassMap<UserData>
    {
        public UserDataMap()
        {
            
            Id(x => x.Id).Column("Id");
            Map(x => x.EmailAddress);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            HasMany(x => x.Interest).Table("UserInterest").KeyColumn("UserID").Inverse().Cascade.All().Not.LazyLoad(); ;
            LazyLoad();
        }
    }

    public class UserInterestMap : ClassMap<UserInterest>
    {
        public UserInterestMap()
        {
            Id(x => x.Id).Column("Id");
            Map(x => x.UserID);
            Map(x => x.Interest);
        }
    }
}
