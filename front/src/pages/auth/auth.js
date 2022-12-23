import { Box, Button, FormHelperText, TextField, Typography } from "@mui/material";
import React, { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { AppContext } from "../../context";
import "./styles.css";

export const setUserData = () => {
  const JWT = localStorage.getItem("userJWT");
  const user = parseJwt(JWT);
  const userRoles = user["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  const name = user["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
  return { token: JWT, roles: [...userRoles], name };
};

export const Auth = () => {
  const navigateTo = useNavigate();
  const [isSignup, setIsSignup] = useState(false);
  const { user, setUser } = useContext(AppContext);

  const [errorMessage, setErrorMessage] = useState("");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  // State variables for input validation
  const [nameError, setNameError] = useState("");
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [formValid, setFormValid] = useState(false);

  const switchBetweenLoginAndRegister = () => {
    setNameError("");
    setEmailError("");
    setPasswordError("");
    setIsSignup(!isSignup);
  };

  const headerText = isSignup ? "Signup" : "Login";

  const handleSubmit = (e) => {
    e.preventDefault();

    // Validate the form before submission
    if (validateForm()) {
      isSignup ? sendRegisterRequest() : sendLoginRequest();
    }
  };

  const validateForm = () => {
    let formIsValid = true;

    // Validate the name field
    if (!name) {
      setNameError("Name cannot be empty");
      formIsValid = false;
    } else {
      setNameError("");
    }

    // Validate the email field
    if (isSignup) {
      const emailRegex =
        /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      if (!emailRegex.test(email)) {
        setEmailError("Please enter a valid email address");
        formIsValid = false;
      } else {
        setEmailError("");
      }
    }

    // Validate the password field
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{6,})/;
    if (!passwordRegex.test(password)) {
      setPasswordError(
        "Password must contain a digit, a lowercase letter, an uppercase letter, a non alpha numeric value, and have a minimum length of 6"
      );
      formIsValid = false;
    } else {
      setPasswordError("");
    }

    setFormValid(formIsValid);
    return formIsValid;
  };

  const sendLoginRequest = async () => {
    const userToLogIn = { userName: name, password };
    try {
      const response = await fetch("https://vlogapidankaz.azurewebsites.net/api/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(userToLogIn),
      });
      const data = await response.json();
      const tokenData = parseJwt(data.accessToken);
      const roles = tokenData["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      const name = tokenData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
      localStorage.setItem(
        "userJWT",
        JSON.stringify({ token: data.accessToken, roles: [roles], name })
      );
      setUser({ token: data.accessToken, roles: [roles], name });
      navigateTo("/");
    } catch (e) {
      console.log(e);
      setErrorMessage("Invalid user information");
    }
  };

  const sendRegisterRequest = async () => {
    const userToSignUp = { userName: name, email, password };
    try {
      const response = await fetch("https://vlogapidankaz.azurewebsites.net/api/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(userToSignUp),
      });
      const data = await response.json();
      setIsSignup(false);
    } catch (e) {
      console.log(e);
    }
  };

  return (
    <div className="auth">
      <form className="auth-form" onSubmit={handleSubmit}>
        <Box
          maxWidth={400}
          margin={"auto"}
          padding={5}
          borderRadius={5}
          boxShadow={"0px 0px 2px 1px rgba(0,0,0,0.75)"}
          sx={{
            ":hover": {
              boxShadow: "0px 0px 4px 2px rgba(0,0,0,0.75)",
            },
            backgroundColor: "#FFFFEF",
          }}
        >
          <Typography variant="h2" padding={3} textAlign="center" sx={{ color: "#121212" }}>
            {headerText}
          </Typography>
          {!!errorMessage && <FormHelperText error>{errorMessage}</FormHelperText>}
          <TextField
            name="name"
            value={name}
            type={"text"}
            variant="outlined"
            placeholder="Name"
            margin="normal"
            sx={{ width: "100%" }}
            onChange={(e) => {
              const newName = e.target.value;
              setName(newName);
            }}
            error={!!nameError} // show an error if there is a non-empty error message
            helperText={nameError} // display the error message
          />
          {isSignup && (
            <TextField
              name="email"
              value={email}
              type={"email"}
              variant="outlined"
              placeholder="Email"
              margin="normal"
              sx={{ width: "100%" }}
              onChange={(e) => {
                const newEmail = e.target.value;
                setEmail(newEmail);
              }}
              error={!!emailError}
              helperText={emailError}
            />
          )}
          <TextField
            name="password"
            value={password}
            type={"password"}
            variant="outlined"
            placeholder="Password"
            margin="normal"
            sx={{ width: "100%" }}
            onChange={(e) => {
              const newPassword = e.target.value;
              setPassword(newPassword);
            }}
            error={!!passwordError}
            helperText={passwordError}
          />
          <Button
            type="submit"
            variant="contained"
            color="warning"
            sx={{ marginTop: 3, borderRadius: 3, width: "100%" }}
          >
            {headerText}
          </Button>
          <Button
            onClick={switchBetweenLoginAndRegister}
            sx={{ marginTop: 3, borderRadius: 3, width: "100%" }}
          >
            Change To {isSignup ? "Login" : "Signup"}
          </Button>
        </Box>
      </form>
    </div>
  );
};

function parseJwt(token) {
  var base64Url = token.split(".")[1];
  var base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
  var jsonPayload = decodeURIComponent(
    window
      .atob(base64)
      .split("")
      .map(function (c) {
        return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join("")
  );

  return JSON.parse(jsonPayload);
}
