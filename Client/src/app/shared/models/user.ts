import { Order } from "./order"

export interface User {
    id: string
    email: string
    firstname: string
    lastname: string
    dateOfBirth: string
    gender: number
    provider: string
    role: string
    isAuthenticated: boolean
    createAt: string
}

export interface UserDetail {
    id: string
    email: string
    firstname: string
    lastname: string
    dateOfBirth: string
    gender: number
    provider: string
    role: string
    isAuthenticated: boolean
    createAt: string
    updateAt: string
    userOrders: Order[]
}