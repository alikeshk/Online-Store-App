﻿import React from 'react';
import { useState, useEffect } from "react";
import "./PaymentPage.css";
import { Row } from "react-bootstrap";
import { Col } from "react-bootstrap";
import { Link } from 'react-router-dom';
import { ProductBox } from "../../components/Product/ProductBox/ProductBox.js";

export const PaymentPage = ({ isLoggedIn, setLoggedIn }) => {
    const [street, setStreet] = useState('');
    const [city, setCity] = useState('');
    const [country, setCountry] = useState('');
    const [postalCode, setPostalCode] = useState('');
    const [firstNameTextBoxValue, setFirstNameTextBoxValue] = useState('');
    const [lastNameTextBoxValue, setLastNameTextBoxValue] = useState('');
    const [cardNumberTextBoxValue, setCardNumberTextBoxValue] = useState('');
    const [cartTotal, setCartTotal] = useState({});
    const [cartProducts, setCartProducts] = useState([]);

    useEffect(() => {
        // Use a function inside useEffect to handle asynchronous code
        const fetchAddressData = async () => {
            try {
                const response = await fetch('address/getaddressforcurrentuser');
                const json = await response.json();
                setStreet(json.street);
                setCity(json.city);
                setCountry(json.country);
                setPostalCode(json.postalCode);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };
        const fetchPaymentData = async () => {
            try {
                const response = await fetch('payment/getpaymentforcurrentuser');
                const json = await response.json();
                setFirstNameTextBoxValue(json.cardFirstName);
                setLastNameTextBoxValue(json.cardLastName);
                setCardNumberTextBoxValue(json.cardNumber);
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };
        const fetchCartData = async () => {
            try {
                const response = await fetch('cart/getcartforcurrentuser');
                const json = await response.json();
                setCartTotal(json.total);
                setCartProducts(json.productToCarts)
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        fetchAddressData();
        fetchPaymentData();
        fetchCartData();
    }, []);

    const totalPrice = "Total Price: " + cartTotal;

    return (
        <div className="outer-container" style={{ paddingTop: '200px' }}>
            <table style={{ width: '100%', marginTop: '10px', borderCollapse: 'collapse' }}>
                <tbody>
                    <Row className="justify-content-md-start">
                        <Col className="mb-3">
                            <label htmlFor="firstName" className="text-right">First Name:</label>
                            <input className="text-field" id="firstName" type="text" value={firstNameTextBoxValue} onChange={(e) => { setFirstNameTextBoxValue(e.target.value) }} style={{ textAlign: 'center' }} />
                        </Col>
                        <Col className="mb-3">
                            <label htmlFor="street" className="text-right">Street:</label>
                            <input className="text-field" id="street" type="text" value={street} onChange={(e) => { setStreet(e.target.value) }} style={{ textAlign: 'center' }} />
                        </Col>
                    </Row>
                    <Row className="justify-content-md-start">
                        <Col className="mb-3">
                            <label for="lastName">Last Name:</label>
                            <input className="text-field" id="lastName" type="text" value={lastNameTextBoxValue} onChange={(e) => { setLastNameTextBoxValue(e.target.value) }} style={{ textAlign: 'center' }} />
                        </Col>
                        <Col className="mb-3">
                            <label for="city">City:</label>
                            <input className="text-field" id="city" type="text" value={city} onChange={(e) => { setCity(e.target.value) }} style={{ textAlign: 'center' }} />
                        </Col>
                    </Row>
                    <Row className="justify-content-md-start">
                        <Col className="mb-3">
                            <label for="cardNumber">Card Number:</label>
                            <input className="text-field" id="cardNumber" type="text" value={cardNumberTextBoxValue} onChange={(e) => { setCardNumberTextBoxValue(e.target.value) }} style={{ textAlign: 'center' }} />
                        </Col>
                        <Col className="mb-3">
                            <label for="country">Country:</label>
                            <input className="text-field" id="country" type="text" value={country} onChange={(e) => { setCountry(e.target.value) }} style={{ textAlign: 'center' }} />
                        </Col>
                    </Row>
                    <Row className="justify-content-md-start">
                        <Col className="mb-3">
                            <label for="postalCode">Postal Code:</label>
                            <input className="text-field" id="postalCode" type="text" value={postalCode} onChange={(e) => { setPostalCode(e.target.value) }} style={{ textAlign: 'center' }} />
                        </Col>
                    </Row>
                    {cartProducts.length > 0 ?
                        cartProducts?.map((product) => (
                            <ProductBox product={product}></ProductBox>
                        ))
                        : <div>Loading...</div>}
                    <h6 className="d-flex justify-content-md-end">{totalPrice}</h6>
                    <div style={{ display: 'flex', justifyContent: 'right' }}>
                        <Link to={`/order-confirmation`}>
                            <button className="btn-primary">Place Order</button>
                        </Link>
                    </div>
                </tbody>
            </table>
        </div>
    );
};