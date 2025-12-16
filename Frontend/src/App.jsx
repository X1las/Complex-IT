import { useState } from 'react'
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css' 
import logo from './assets/image.png';
import icon from './assets/icon.png';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext.jsx';
import Container from 'react-bootstrap/Container';
import { Navbar, Nav, NavDropdown } from 'react-bootstrap';
import Dropdown from 'react-bootstrap/Dropdown';
import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

function App() {

  const [search, setSearch] = useState('');
  const navigate = useNavigate();
  const { user } = useAuth();
  const { logout } = useAuth();
  const handleLogout = () => { logout(); };

  const onSearchSubmit = (e) => {
    e.preventDefault();
    if (!search.trim()) return;
    navigate(`/search/${(search)}`);                                                                                                                   
  }

  return (
    <>
      <Navbar fixed="top" className="navbar" >

          <Container fluid className="justify-content-evenly" >

          <Navbar.Brand as={Link} to="/" className="d-flex align-items-center">
          
            <img src={logo} className="logo" alt="Logo" />
          </Navbar.Brand>

          
          <Form onSubmit={onSearchSubmit} className="flex-grow-1 mx-3" >

            <Row className="g-2">

              <Col style={{  justifyContent: "center", display: "flex" }}>

                <Form.Control
                  className="form-control searchField"
                  placeholder="Search"
                  style={{color:'white'}}
                  type="search"
                  aria-label="Search"
                  value={search}
                  onChange={e => setSearch(e.target.value)}
                />
              </Col>

            </Row>

          </Form>

          <Nav >

            <NavDropdown menuVariant="dark" title={<img src={icon} alt="profilePic" className="profileIcon" />} id="nav-dropdown" align="end" style={{  marginLeft: "100px"}}>
                   
              <NavDropdown.Item  as={Link} to={user ? `/profile/${user.username}` : `/login`} ><button style={{width:"100%", margin:0}} className="btn btn-outline-warning" >Your Page</button></NavDropdown.Item>
                { !user ? null:
                <NavDropdown.Item ><button style={{width:"100%", margin:0}} className="btn btn-outline-warning"  onClick={handleLogout}>Logout</button></NavDropdown.Item>
                }
              <NavDropdown.Item as={Link} to={`/`} ><button style={{width:"100%", margin:0}} className="btn btn-outline-warning">Andet?</button></NavDropdown.Item>

            </NavDropdown>
          </Nav>

        </Container>

      </Navbar>

      <Outlet />
    </>
  )
}

export default App
