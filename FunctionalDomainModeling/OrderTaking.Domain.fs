﻿namespace OrderTaking.Domain

open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open FSharpx.Collections

type Undefined = exn

// Product code related
type WidgetCode = WidgetCode of string
 // constraint: starting with "W" then 4 digits
type GizmoCode = GizmoCode of string
 // constraint: starting with "G" then 3 digits
type ProductCode =
    | Widget of WidgetCode
    | Gizmo of GizmoCode
module ProductCode =
    let create str = Widget (WidgetCode "abc")

// Order quantity related
type UnitQuantity = private UnitQuantity of int
 // between 1 and 1000
type KilogramQuantity = private KilogramQuantity of decimal<kg>
 // between 0.05 and 100.00
type OrderQuantity =
    | Unit of UnitQuantity
    | Kilogram of KilogramQuantity

type String50 = private String50 of string
module String50 =
    let create str = (String50 str)
    let createOption str = Some (String50 str)
        
type EmailAddress = private EmailAddress of string
module EmailAddress =
    let create str = (EmailAddress str)

type OrderId = private OrderId of string
module OrderId =
    let create str =
        if System.String.IsNullOrEmpty(str) then
            failwith "OrderId must not be null or empty"
        elif str.Length > 50 then
            failwith "OrderId must not be more than 50 chars"
        else
            OrderId str

    let value (OrderId str) = str

type OrderLineId = private OrderLineId of int
module OrderLineId =
    let create int = (OrderLineId int)

type CustomerId = Undefined

type PersonalName = {
    FirstName : String50
    LastName : String50 }

type CustomerInfo = {
    Name : PersonalName
    EmailAddress : EmailAddress }

type ZipCode = private ZipCode of string
module ZipCode =
    let create str = (ZipCode str)

type UnvalidatedCustomerInfo = {
    FirstName : string
    LastName : string
    EmailAddress : string }

type Address = {
    AddressLine1 : String50
    AddressLine2 : String50 option
    AddressLine3 : String50 option
    AddressLine4 : String50 option
    City : String50
    ZipCode : ZipCode }

type ShippingAddress = Address
type BillingAddress = Address
type Price = Undefined
type BillingAmount = Undefined

type Order = {
    Id : OrderId // id for entity
    CustomerId : CustomerId // customer reference
    ShippingAddress : ShippingAddress
    BillingAddress : BillingAddress
    OrderLines : NonEmptyList<OrderLine>
    AmountToBill : BillingAmount }

and OrderLine = {
    Id : OrderLineId // id for entity
    OrderId : OrderId // order reference
    ProductCode : ProductCode
    OrderQuantity : OrderQuantity
    Price : Price }

type UnvalidatedAddress = {
    AddressLine1 : string
    AddressLine2 : string
    AddressLine3 : string
    AddressLine4 : string
    City : string
    ZipCode : string }

type CheckedAddress = CheckedAddress of UnvalidatedAddress

type CheckProductCodeExists = ProductCode -> bool

type UnvalidatedOrderLine = {
    OrderLineId : int
    ProductCode : string
    Quantity : decimal }

type UnvalidatedOrder = {
    OrderId : string
    CustomerInfo : UnvalidatedCustomerInfo
    ShippingAddress : UnvalidatedAddress
    OrderLines : UnvalidatedOrderLine list }

type CheckAddressExists =
    UnvalidatedAddress -> CheckedAddress

type ValidatedOrderLine = {
    OrderLineId : OrderLineId
    ProductCode : ProductCode
    Quantity : OrderQuantity }

type ValidatedOrder = {
    OrderId : OrderId
    CustomerInfo : CustomerInfo
    ShippingAddress : Address
    OrderLines : ValidatedOrderLine list }

type PlaceOrderEvents = {
    AcknowledgementSent : Undefined
    OrderPlaced : Undefined
    BillableOrderPlaced : Undefined }

type PlaceOrderError =
    | ValidationError of ValidationError list

and ValidationError = {
    FieldName : string
    ErrorDescription : string }

type PlaceOrder =
    UnvalidatedOrder -> Result<PlaceOrderEvents,PlaceOrderError>

module KilogramQuantity =

    /// decimal -> Result<KilogramQuantity,string>
    let create qty =
        if qty < 0.05m then
            Error "KilogramQuantity cannot be less than 0.05"
        else if qty > 100.0m then
            Error "KilogramQuantity cannot be more than 100.0"
        else
            Ok (KilogramQuantity (qty * 1.0m<kg>))

    let value (KilogramQuantity qty) = qty

module UnitQuantity =

    /// int -> Result<UnitQuantity,string>
    let create qty =
        if qty < 1 then
            Error "UnitQuantity cannot be negative"
        else if qty > 1000 then
            Error "UnitQuantity cannot be more than 1000"
        else
            Ok (UnitQuantity qty)

    let value (UnitQuantity qty) = qty
