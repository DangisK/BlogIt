import "./App.css";
import { AppContext } from "./context";
import { Route, Routes, useNavigate, Navigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { Auth } from "./pages/auth";
import { Feed, Navbar } from "./components";
import { Sidebar } from "./components/sidebar/sidebar";
import { Rightbar } from "./components/rightbar/rightbar";
import { Stack } from "@mui/system";
import { Box, createTheme, ThemeProvider } from "@mui/material";
import { AddPostModal } from "./components/feed/add-post-modal";
import { Post } from "./components/post";

function App() {
  const localStorageData = JSON.parse(localStorage.getItem("userJWT"));
  const [user, setUser] = useState(
    localStorageData === null
      ? { token: "", roles: [], name: "" }
      : {
          token: localStorageData.token,
          roles: [...localStorageData.roles].flat(),
          name: localStorageData.name,
        }
  );
  const [isUserLoggedIn, setIsUserLoggedIn] = useState(user.token !== "");
  const navigateTo = useNavigate();
  const [mode, setMode] = useState("dark");

  const darkTheme = createTheme({
    palette: {
      mode: mode,
    },
  });

  const onLogout = () => {
    localStorage.removeItem("userJWT");
    setUser({ token: "", roles: [], name: "" });
    setIsUserLoggedIn(false);
  };

  useEffect(() => {
    if (!isUserLoggedIn) {
      setMode("light");
    } else {
      setMode("dark");
    }
  }, [isUserLoggedIn]);

  useEffect(() => {
    setIsUserLoggedIn(user.token !== "");
  }, [user]);

  const ProtectedRoute = ({ children }) => {
    if (user.token === "") {
      // user is not authenticated
      return <Navigate to="/auth" />;
    }
    return children;
  };

  return (
    <>
      <AppContext.Provider value={{ user, setUser }}>
        <ThemeProvider theme={darkTheme}>
          <Box bgcolor={"background.default"} color={"text.primary"} sx={{ minHeight: "100vh" }}>
            {isUserLoggedIn && <Navbar onLogout={onLogout} setMode={setMode} mode={mode} />}
            <Stack direction="row" spacing={2} justifyContent="space-between">
              {isUserLoggedIn && <Sidebar setMode={setMode} mode={mode} onLogout={onLogout} />}
              <Routes>
                {!isUserLoggedIn && <Route path="/auth" element={<Auth />} />}
                <Route
                  exact
                  path="/"
                  element={
                    <ProtectedRoute>
                      <Feed />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/posts/:postId"
                  element={
                    <ProtectedRoute>
                      <Post />
                    </ProtectedRoute>
                  }
                />
                {isUserLoggedIn && <Route path="*" element={<Feed />}></Route>}
              </Routes>
              {isUserLoggedIn && <Rightbar />}
            </Stack>
          </Box>
        </ThemeProvider>
      </AppContext.Provider>
    </>
  );
}

export default App;
