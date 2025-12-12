import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Container, Form, Button, Alert, Card, InputGroup } from 'react-bootstrap';
import '../css/register.css';
import { NL_API } from './search.jsx';

const Register = () => {
  const [formData, setFormData] = useState({ 
    username: '', 
    password: '', 
    confirmPassword: '' 
  });
  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [apiError, setApiError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const navigate = useNavigate();
  const { user } = useAuth();

  // Redirect if already logged in
  useEffect(() => {
    if (user) {
      navigate('/');
    }
  }, [user, navigate]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
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

    // Username validation
    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    } else if (formData.username.length < 4) {
      newErrors.username = 'Username must be at least 4 characters';
    } else if (!/^[a-zA-Z0-9_]+$/.test(formData.username)) {
      newErrors.username = 'Username can only contain letters, numbers, and underscores';
    }

    // Password validation
    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 8) {
      newErrors.password = 'Password must be at least 8 characters';
    } else if ((formData.password.match(/[a-zA-Z]/g) || []).length < 3) {
      newErrors.password = 'Password must contain at least 3 letters';
    } else if ((formData.password.match(/[0-9]/g) || []).length < 2) {
      newErrors.password = 'Password must contain at least 2 numbers';
    } else if (formData.username && formData.password.toLowerCase().includes(formData.username.toLowerCase())) {
      newErrors.password = 'Password cannot contain your username';
    }

    // Confirm password validation
    if (!formData.confirmPassword) {
      newErrors.confirmPassword = 'Please confirm your password';
    } else if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setApiError('');
    setSuccessMessage('');

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      console.log('Attempting registration for:', formData.username);
      console.log('API URL:', `${NL_API}/api/users/create`);
      // hvis det ikke fungere sådan her skal der stå ${NL_API}/api/users/login i fetch
      const response = await fetch(`${NL_API}/api/users/create`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: formData.username,
          password: formData.password
        }),
        credentials: 'include'
      });

      console.log('Response status:', response.status);
      const data = await response.json();
      console.log('Response data:', data);

      if (!response.ok) {
        throw new Error(data.error || data.Error || data.message || 'Registration failed');
      }

      // Show success message
      setSuccessMessage('Registration successful! Redirecting to login...');
      
      // Clear form
      setFormData({ username: '', password: '', confirmPassword: '' });
      
      // Redirect to login page after 2 seconds
      setTimeout(() => {
        navigate('/login');
      }, 2000);
      
    } catch (error) {
      console.error('Registration error:', error);
      setApiError(error.message || 'An error occurred during registration. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="register-container">
      <Card style={{ width: '100%', maxWidth: '450px' }} className="shadow">
        <Card.Body className="p-4">
          <h1 className="text-center mb-4">Register</h1>
          
          {successMessage && (
            <Alert variant="success" dismissible onClose={() => setSuccessMessage('')}>
              {successMessage}
            </Alert>
          )}

          {apiError && (
            <Alert variant="danger" dismissible onClose={() => setApiError('')}>
              {apiError}
            </Alert>
          )}

          <Form onSubmit={handleRegister}>
            <Form.Group className="mb-3" controlId="username">
              <Form.Label>Username</Form.Label>
              <Form.Control
                type="text"
                name="username"
                value={formData.username}
                onChange={handleInputChange}
                disabled={isLoading}
                isInvalid={!!errors.username}
                autoComplete="username"
                placeholder="Choose a username"
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
                value={formData.password}
                onChange={handleInputChange}
                disabled={isLoading}
                isInvalid={!!errors.password}
                autoComplete="new-password"
                placeholder="Create a password"
              />
              {errors.password && (
                <Form.Control.Feedback type="invalid" style={{ display: 'block' }}>
                  {errors.password}
                </Form.Control.Feedback>
              )}
            </Form.Group>

            <Form.Group className="mb-3" controlId="confirmPassword">
              <Form.Label>Confirm Password</Form.Label>
              <Form.Control
                type={showPassword ? "text" : "password"}
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleInputChange}
                disabled={isLoading}
                isInvalid={!!errors.confirmPassword}
                autoComplete="new-password"
                placeholder="Confirm your password"
              />
              {errors.confirmPassword && (
                <Form.Control.Feedback type="invalid" style={{ display: 'block' }}>
                  {errors.confirmPassword}
                </Form.Control.Feedback>
              )}
            </Form.Group>

            <Form.Group className="mb-3" controlId="showPassword">
              <Form.Check
                type="checkbox"
                label="Show passwords"
                checked={showPassword}
                onChange={(e) => setShowPassword(e.target.checked)}
                disabled={isLoading}
              />
            </Form.Group>

            <Button 
              variant="success"
              type="submit" 
              disabled={isLoading}
              className="w-100"
              size="lg"
            >
              {isLoading ? 'Registering...' : 'Register'}
            </Button>
          </Form>

          <div className="text-center mt-3">
            <p className="mb-0">
              Already have an account? <Link to="/login">Login here</Link>
            </p>
          </div>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default Register;