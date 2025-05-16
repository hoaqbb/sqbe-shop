export interface OrderRequest {
  fullname: string;
  email: string;
  phoneNumber: string;
  address: string;
  deliveryMethod: number;
  paymentMethod: string;
  shippingFee: number;
  amount: number;
  subtotal: number;
  discountAmount: number;
  promotionCode: string;
  note: string;
}
