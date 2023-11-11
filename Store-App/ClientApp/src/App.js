import { Route, Routes } from 'react-router-dom';
import './custom.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useState } from 'react';
import { LoginPage } from './pages/LoginPage/LoginPage';
import { APITest } from './components/APITest/APITest';
import { DisplayProductBoxes } from './components/Product/ProductBox/ProductBox';
import { NavMenu } from './components/NavMenu/NavMenu';
import { Container } from 'reactstrap';

export default function App() {

    const [isLoggedIn, setIsLoggedIn] = useState(false);

    return (
        <div>
            <NavMenu />
            <Container>
                <Routes>
                    <Route path="/" element={<DisplayProductBoxes />} />
                    <Route path="/apitest" element={<APITest />} />
                    <Route path="/login" element={<LoginPage />} />
                </Routes>
            </Container>
        </div>
    );
}
