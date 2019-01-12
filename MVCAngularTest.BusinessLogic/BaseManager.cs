using MVCAngularTest.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAngularTest.DataAccess
{
    public abstract class BaseManager
    {
        protected string LIST_KEY = string.Empty;
        protected string BY_ID_KEY = string.Empty;
        private IUnitOfWork _unitOfWork;
        public BaseManager(IUnitOfWork unitOfWork) {
            this._unitOfWork = unitOfWork;
            this.LIST_KEY = this.GetType().Name + "_LIST";
            this.BY_ID_KEY = this.GetType().Name + "_BY_ID_{0}";
        }
        public void Dispose()
        {
            this._unitOfWork.Dispose();
        }
        public abstract void ResetCache();
    }
}
