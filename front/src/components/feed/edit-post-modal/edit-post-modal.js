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

export const EditPostModal = ({ post, onUpdate, close }) => {
  const { user } = useContext(AppContext);
  const [isLoading, setIsLoading] = useState(false);
  const [postName, setPostName] = useState(post.name);
  const [postBody, setPostBody] = useState(post.body);

  const updatePost = async () => {
    setIsLoading(true);
    const updatedPost = { Name: postName, Body: postBody };
    try {
      const response = await fetch(`https://vlogapidankaz.azurewebsites.net/api/posts/${post.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(updatedPost),
      });
      const data = await response.json();
      onUpdate(data);
      close();
    } catch (e) {
      console.log(e);
    }
    setIsLoading(false);
  };

  return (
    <>
      <StyledModal open={true} onClose={close}>
        <Box
          width={440}
          height={370}
          bgcolor={"background.default"}
          color={"text.primary"}
          p={3}
          borderRadius={5}
        >
          <Typography variant="h6" textAlign="center">
            Edit Post
          </Typography>
          <UserBox sx={{ marginTop: "15px" }}>
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
            value={postName}
            placeholder="Title"
            onChange={(e) => {
              setPostName(e.target.value);
            }}
          />
          <TextField
            sx={{ width: "100%", marginTop: "15px" }}
            label="What's on your mind?"
            multiline
            maxRows={3}
            value={postBody}
            variant="standard"
            placeholder="Post"
            onChange={(e) => {
              setPostBody(e.target.value);
            }}
          />
          <ButtonGroup variant="contained" fullWidth sx={{ marginTop: "15px" }}>
            <Button disabled={postName === "" || postBody === ""} onClick={updatePost}>
              Update
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
