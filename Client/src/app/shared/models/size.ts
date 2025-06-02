export interface Size {
    id: number
    name: string
}

export interface SizeVariant {
    id: number
    size: string
    quantity: number
}

export interface SizeDetail {
    id: number
    name: string
    createAt: string
    updateAt: string
    productCount: number
}