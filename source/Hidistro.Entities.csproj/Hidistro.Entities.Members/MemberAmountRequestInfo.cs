using System;

namespace Hidistro.Entities.Members
{
	public class MemberAmountRequestInfo
	{
		public int Id
		{
			get;
			set;
		}

		public int UserId
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public System.DateTime RequestTime
		{
			get;
			set;
		}

		public decimal Amount
		{
			get;
			set;
		}

		public RequesType RequestType
		{
			get;
			set;
		}

		public string AccountCode
		{
			get;
			set;
		}

		public string AccountName
		{
			get;
			set;
		}

		public string BankName
		{
			get;
			set;
		}

		public string Remark
		{
			get;
			set;
		}

		public RequesState State
		{
			get;
			set;
		}

		public System.DateTime? CheckTime
		{
			get;
			set;
		}

		public string CellPhone
		{
			get;
			set;
		}

		public string Operator
		{
			get;
			set;
		}

		public string RedpackId
		{
			get;
			set;
		}
	}
}
