import { ProductVariant } from "./product"

export interface Cart {
    id: string
    cartItems: CartItem[]
}

export interface CartItem {
    id: number
    name: string
    price: number
    photo: string
    discount: number
    quantity: number
    slug: string
    category: string
    productVariant: ProductVariant
}