export interface PromotionDetail {
  id: number;
  code: string;
  description: string;
  discountType: number;
  discountValue: number;
  minOrderAmount: number;
  usageLimit: number;
  usageCount: number;
  isActive: boolean;
  isUserRestricted: boolean;
  validateFrom: string;
  validateTo: string;
  createAt: string;
  updateAt: string;
  maxDiscountValue: number;
}

export interface CreatePromotion {
  code: string;
  description: string;
  discountType: number;
  discountValue: number;
  minOrderAmount: number;
  usageLimit: number;
  isActive: boolean;
  isUserRestricted: boolean;
  validateFrom: string;
  validateTo: string;
  maxDiscountValue: number;
}

export interface UpdatePromotion {
  code: string;
  description: string;
  discountType: number;
  discountValue: number;
  minOrderAmount: number;
  usageLimit: number;
  isActive: boolean;
  isUserRestricted: boolean;
  validateFrom: string;
  validateTo: string;
  maxDiscountValue: number;
}
