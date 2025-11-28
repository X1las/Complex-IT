import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import {BrowserRouter,Routes, Route} from 'react-router-dom'
import './index.css'
import App from './App.jsx'
import Home from './home.jsx'
import Profile from './routes/profile.jsx'
import Bookmarks from './routes/Bookmarks.jsx'
import History from './routes/History.jsx'
import Ratings from './routes/Ratings.jsx'

const root = createRoot(document.getElementById('root'))
root.render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />}>
          
           <Route path="profile/:id/" element={<Profile/>}>
           
        </Route>
          {/*  <Route path="bookmarks" element={<Bookmarks/>} />
            <Route path="history" element={<History/>} />
            <Route path="ratings" element={<Ratings/>} />
          </Route>

          <Route path="login" element={<Login />}>
            <Route path="Register" element={<Register />} />
          </Route>
          
          <Route path="person/:id" element={<Person />} />
          <Route path="title/:id" element={<Title />} />
          <Route path="search" element={<Search />} /> */}

          <Route path="*" element={<div>404 Not Found</div>} />

        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)
