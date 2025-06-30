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

export interface Order {
  id: string;
  amount: number;
  status: number;
  createAt: string;
  updateAt: any;
  address: string;
  fullname: string;
  phoneNumber: string;
  paymentMethod: string;
}

export interface OrderDetail {
  id: string
  amount: number
  subtotal: number
  discountAmount: number
  shippingFee: number
  fullname: string
  email: string
  address: string
  phoneNumber: string
  note: string
  status: number
  deliveryMethod: number
  createAt: string
  updateAt: any
  paymentInfo: PaymentInfo
  orderItems: OrderItem[]
}

export interface OrderItem {
  id: number
  quantity: number
  productName: string
  productColor: string
  productSize: string
  price: number
  discount: number
  productImageUrl: string
}

export interface PaymentInfo {
  method: string
  provider: string
  amount: number
  createAt: string
  updateAt: string
  transactionId: string
  userId: any
  status: boolean
  currencyCode: string
}

export enum OrderStatus {
  Pending = 0,
  Confirmed = 1,
  Shipping = 2,
  Completed = 3,
  Cancelled = 4
}
