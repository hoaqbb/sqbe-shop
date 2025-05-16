export class ProductFilterParams {
    category = ''
    colors: number[] = []
    sizes: number[] = []
    priceFrom: number
    priceTo: number
    promotion = false
    sort: string
    pageNumber = 1
    pageSize = 8
}

export class ProductSearchParams {
    keyword = ''
    pageNumber = 1
    pageSize = 8
}