export type Pagination<T> = {
    pageIndex: number;
    pageSize: number;
    totalPages: number;
    totalCount: number;
    data: T[]
}