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
  }