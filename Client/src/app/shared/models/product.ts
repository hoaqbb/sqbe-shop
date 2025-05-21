import { Category } from "./category"
import { Color } from "./color"

export interface Product {
    id: string
    name: string
    price: number
    mainPhoto: string
    subPhoto: string
    productColors: Color[]
    discount: number
    isVisible: boolean
    slug: string
    category: string
    createAt: string
    isLikedByCurrentUser: boolean
  }

  export interface ProductDetail {
    id: string
    name: string
    price: number
    description: string
    discount: number
    slug: string
    category: Category
    productImages: ProductImage[]
    productVariants: ProductVariant[]
  }

  export interface ProductImage {
    id: number
    imageUrl: string
    isMain: boolean
    isSub: boolean
  } 
  
  export interface ProductVariant {
    id: number
    quantity: number
    color: string
    colorCode: string
    size: string
  }

  export interface CreateProduct {
    name: string
    price: number
    description: string
    discount: number
    categoryId: number
    productColors: number[]
    productSizes: number[]
    mainImage: File
    subImage: File
    productImages: File[]
  }

  export interface UpdateProduct {
  name: string
  price: number
  description: string
  discount: number
  categoryId: number
  productColors: number[]
  productSizes: number[]
}