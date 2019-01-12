using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVCAngularTest.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract partial class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [NotMapped]
        public int UniqueId { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            return Equals(obj as BaseEntity);
        }
        //[NotMapped]
        //public DateTime? CreatedDate { get; set; }

        public override int GetHashCode()
        {
            if (Equals(
                UniqueId, default(int)))
                return base.GetHashCode();
            return UniqueId.GetHashCode();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }


    }
}
