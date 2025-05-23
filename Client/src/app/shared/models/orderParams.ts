export class OrderFilterParams {
    sort: string
    status: number
    amountFrom: number
    amountTo: number
    isDiscounted: boolean
    paymentMethod: string
    pageNumber = 1
    pageSize = 8
}