import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Container, Form, Button, Alert, Card } from 'react-bootstrap';
import '../css/login.css';
import { NL_API } from './search';

const Login = () => {
  const [credentials, setCredentials] = useState({ username: '', password: '' });
  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [apiError, setApiError] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const navigate = useNavigate();
  const { user, login } = useAuth();

  // Redirect if already logged in
  useEffect(() => {
    if (user) {
      navigate('/');
    }
  }, [user, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setCredentials(prev => ({
      ...prev,
      [name]: value
    }));
    // Clear error for this field when user types
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!credentials.username.trim()) {
      newErrors.username = 'Username is required';
    }

    if (!credentials.password) {
      newErrors.password = 'Password is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    setApiError('');

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      const response = await fetch(`${NL_API}/api/users/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: credentials.username,
          password: credentials.password
        }),
        credentials: 'include'
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.error || data.Error || data.message || 'Login failed');
      }

      // Store user info using auth context
      const userData = { username: credentials.username };
      login(userData, data.token);

      // Redirect to home page
      navigate('/');
      
    } catch (error) {
      setApiError(error.message || 'An error occurred during login. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="login-container" style={{ padding: '75px' }}>
      <Card>
        <Card.Body className="p-4">
          <h1 className="text-center mb-4">Login</h1>
          
          {apiError && (
            <Alert variant="danger" dismissible onClose={() => setApiError('')}>
              {apiError}
            </Alert>
          )}

          <Form onSubmit={handleLogin}>
            <Form.Group className="mb-3" controlId="username">
              <Form.Label>Username</Form.Label>
              <Form.Control
                type="text"
                name="username"
                value={credentials.username}
                onChange={handleInputChange}
                disabled={isLoading}
                isInvalid={!!errors.username}
                placeholder="Enter username"
              />
              {errors.username && (
                <Form.Control.Feedback type="invalid" style={{ display: 'block' }}>
                  {errors.username}
                </Form.Control.Feedback>
              )}
            </Form.Group>

            <Form.Group className="mb-3" controlId="password">
              <Form.Label>Password</Form.Label>
              <Form.Control
                type={showPassword ? "text" : "password"}
                name="password"
                value={credentials.password}
                onChange={handleInputChange}
                disabled={isLoading}
                isInvalid={!!errors.password}
                placeholder="Enter password"
              />
              {errors.password && (
                <Form.Control.Feedback type="invalid" style={{ display: 'block' }}>
                  {errors.password}
                </Form.Control.Feedback>
              )}
            </Form.Group>

            <Form.Group className="mb-3" controlId="showPassword">
              <Form.Check
                type="checkbox"
                label="Show password"
                checked={showPassword}
                onChange={(e) => setShowPassword(e.target.checked)}
                disabled={isLoading}
              />
            </Form.Group>

            <Button
              variant="btn-outline-warning"
              type="submit" 
              disabled={isLoading}
              className="w-100 btn btn-outline-warning"
              size="lg"
            >
              {isLoading ? 'Logging in...' : 'Login'}
            </Button>
          </Form>

          <div className="text-center mt-3">
            <p className="mb-0">
              Don't have an account? <Link to="/register">Register here</Link>
            </p>
          </div>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Login;