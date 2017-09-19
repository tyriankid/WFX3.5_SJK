using Hidistro.Entities.Store;
using System;

namespace Hidistro.ControlPanel.Store
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Delegate, Inherited=true, AllowMultiple=true)]
	public class PrivilegeCheckAttribute : Attribute
	{
		private Hidistro.Entities.Store.Privilege privilege;

		public Hidistro.Entities.Store.Privilege Privilege
		{
			get
			{
				return this.privilege;
			}
		}

		public PrivilegeCheckAttribute(Hidistro.Entities.Store.Privilege privilege)
		{
			this.privilege = privilege;
		}
	}
}