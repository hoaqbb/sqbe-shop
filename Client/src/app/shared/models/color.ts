import { SizeVariant } from "./size"

export interface Color {
    id: number
    name: string
    colorCode: string
}

export interface ColorVariant {
    color: string
    colorCode: string
    sizeVariants?: SizeVariant[]
}