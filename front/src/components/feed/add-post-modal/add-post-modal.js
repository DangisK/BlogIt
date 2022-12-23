import React, { useContext } from "react";
import {
  Avatar,
  Button,
  ButtonGroup,
  Fab,
  Modal,
  styled,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import { Add } from "@mui/icons-material";
import { Box } from "@mui/system";
import { useState } from "react";
import { AppContext } from "../../../context";
import { stringToColor } from "../../../utils/utils";

export const AddPostModal = ({ onCreate }) => {
  const { user } = useContext(AppContext);
  const [open, setOpen] = useState(false);
  const [postName, setPostName] = useState(null);
  const [postBody, setPostBody] = useState(null);

  const createPost = async () => {
    const createdPost = { Name: postName, Body: postBody };
    try {
      const response = await fetch("https://vlogapidankaz.azurewebsites.net/api/posts", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(createdPost),
      });
      const data = await response.json();
      onCreate(data);
      setOpen(false);
    } catch (e) {
      console.log(e);
    }
  };

  return (
    <>
      <Tooltip
        onClick={(e) => setOpen(true)}
        title="Add post"
        sx={{ position: "fixed", bottom: 20, left: { xs: "calc(50% - 25px)", sm: 30 } }}
      >
        <Fab color="primary">
          <Add />
        </Fab>
      </Tooltip>
      <StyledModal
        open={open}
        onClose={(e) => {
          setOpen(false);
          setPostBody(null);
        }}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box
          width={440}
          height={370}
          bgcolor={"background.default"}
          color={"text.primary"}
          p={3}
          borderRadius={5}
        >
          <Typography variant="h6" textAlign="center">
            Create Post
          </Typography>
          <UserBox>
            <Avatar sx={{ bgcolor: `${stringToColor(user.name.toUpperCase())}` }}>
              {user.name.charAt(0).toUpperCase()}
            </Avatar>
            <Typography fontWeight={500} variant="span" sx={{ wordWrap: "break-word" }}>
              {user.name}
            </Typography>
          </UserBox>
          <TextField
            sx={{ width: "100%" }}
            label="Title"
            multiline
            maxRows={1}
            placeholder="Title"
            onChange={(e) => {
              setPostName(e.target.value);
            }}
          />
          <TextField
            sx={{ width: "100%", marginTop: "20px", marginBottom: "30px" }}
            label="What's on your mind?"
            multiline
            maxRows={3}
            variant="standard"
            placeholder="Post"
            onChange={(e) => {
              setPostBody(e.target.value);
            }}
          />
          <ButtonGroup variant="contained" fullWidth>
            <Button onClick={createPost} disabled={postName === "" || postBody === ""}>
              Post
            </Button>
          </ButtonGroup>
        </Box>
      </StyledModal>
    </>
  );
};

const StyledModal = styled(Modal)({
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
});

const UserBox = styled(Box)({
  display: "flex",
  gap: "10px",
  alignItems: "center",
  marginBottom: "20px",
});
