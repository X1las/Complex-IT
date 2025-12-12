import { useState } from 'react'
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css' 
import logo from './assets/image.png';
import icon from './assets/icon.png';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext.jsx';
import Container from 'react-bootstrap/Container';
import { Navbar, Nav, NavDropdown } from 'react-bootstrap';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

function App() {

  const [search, setSearch] = useState('');
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  /* console.log('Current user in App component:', user); */
  const onSearchSubmit = (e) => {
    e.preventDefault();
    if (!search.trim()) return;
    navigate(`/search/${(search)}`);                                                                                                                   
  }

  return (
    <>
      <Navbar fixed="top"  style={{  backgroundColor: "rgba(26, 26, 26, 0.8)", padding: 0}} >
        <Container fluid className="justify-content-between">
          <Navbar.Brand as={Link} to="/" className="d-flex align-items-center">
            <img src={logo} style={{height:"60px"}} alt="Logo" />
          </Navbar.Brand>

          
          <Form onSubmit={onSearchSubmit} className="flex-grow-1 mx-3">
            <Row className="g-2">
              <Col>
                <Form.Control
                  className="searchField"
                   class="form-control"
                  placeholder="Search"
                  type="search"
                  aria-label="Search"
                  value={search}
                  onChange={e => setSearch(e.target.value)}
                />
              </Col>
            </Row>

          <Nav>
            <NavDropdown  title={<img className="profileIcon" src={icon} alt="profilePic" />} id="nav-dropdown" align="end" >
              <NavDropdown.Item  as={Link} to={user ? `/profile/${user.username}` : `/login`} ><button class="btn btn-outline-dark">Your Page</button></NavDropdown.Item>
              { !user ? null:
              <NavDropdown.Item ><button class="btn btn-outline-dark"  onClick={handleLogout}>Logout</button></NavDropdown.Item>
              }
              <NavDropdown.Item as={Link} to={`/`} ><button class="btn btn-outline-dark">Andet?</button></NavDropdown.Item>
            </NavDropdown>
          </Nav>
        </Container>
      </Navbar>

     
        <Outlet />
    </>
  )
}

export default App

