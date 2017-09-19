using Hidistro.Core.Entities;
using Hidistro.Entities.Comments;
using Hidistro.SqlDal.Comments;
using System;
using System.Collections.Generic;

namespace Hidistro.ControlPanel.Sales
{
	public sealed class ProductCommentHelper
	{
		private ProductCommentHelper()
		{
		}

		public static int DeleteProductConsultation(int consultationId)
		{
			return (new ProductConsultationDao()).DeleteProductConsultation(consultationId);
		}

		public static int DeleteProductReview(long reviewId)
		{
			return (new ProductReviewDao()).DeleteProductReview(reviewId);
		}

		public static int DeleteReview(IList<long> reviews)
		{
			int num;
			if ((reviews == null ? false : reviews.Count != 0))
			{
				int num1 = 0;
				foreach (long review in reviews)
				{
					(new ProductReviewDao()).DeleteProductReview(review);
					num1++;
				}
				num = num1;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static DbQueryResult GetConsultationProducts(ProductConsultationAndReplyQuery consultationQuery)
		{
			return (new ProductConsultationDao()).GetConsultationProducts(consultationQuery);
		}

		public static ProductConsultationInfo GetProductConsultation(int consultationId)
		{
			return (new ProductConsultationDao()).GetProductConsultation(consultationId);
		}

		public static ProductReviewInfo GetProductReview(int reviewId)
		{
			return (new ProductReviewDao()).GetProductReview(reviewId);
		}

		public static DbQueryResult GetProductReviews(ProductReviewQuery reviewQuery)
		{
			return (new ProductReviewDao()).GetProductReviews(reviewQuery);
		}

		public static bool ReplyProductConsultation(ProductConsultationInfo productConsultation)
		{
			return (new ProductConsultationDao()).ReplyProductConsultation(productConsultation);
		}
	}
}