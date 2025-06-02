export interface Category {
    id: number
    name: string
    slug: string
}

export interface CategoryDetail extends Category {
    createAt: string
    updateAt: string
    productCount: number
}

export interface CreateCategory {
    name: string
    slug: string
}