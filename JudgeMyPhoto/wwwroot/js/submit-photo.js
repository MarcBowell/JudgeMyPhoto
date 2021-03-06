﻿// Obtain the root
const rootElement = document.getElementById('root')

// Create a ES6 class components
class UploadImageComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { photoImage: "", uploadProgress: 0, loadingImage: true };

        const url = `../GetPreviewPhoto?cId=${this.props.categoryId}&pId=${this.props.photoNumber}`;
        fetch(url)
            .then((response) => {
                return response.json()
            })
            .then((data) => {
                console.log(data);
                this.setState({ photoImage: data.result, loadingImage: false });
            });
    }

    render() {
        let imagePreview;
        let actionButton;
        if (this.state.photoImage == "") {
            if (this.state.uploadProgress == 0 && !this.state.loadingImage) {
                imagePreview = <p>No photo submitted</p>
                actionButton = <button type="button" class="btn action-upload" onClick={() => this.fileInput.click()}>Select Photo</button>
            }
            else {
                if (this.state.loadingImage)
                    imagePreview = <p>Loading image...</p>
                else
                    imagePreview = <p>Uploading: {this.state.uploadProgress}%</p>
                actionButton = <div></div>
            }
        }
        else {
            imagePreview = <img src={this.state.photoImage}></img>
            actionButton = <button type="button" class="btn action-remove" onClick={() => this.removePhotoClick()}>Remove Photo</button>
        }

        return (
            <div className="col-4-sm photo-submit-area ">
                <div className="row photo-submit-area-title">
                    <div className="col d-flex justify-content-center">
                        Photo {this.props.photoNumber}
                    </div>
                </div>
                <div className="row photo-submit-area-body">
                    <div className="col d-flex justify-content-center">
                        {imagePreview}
                    </div>
                </div>
                <div className="row photo-submit-area-action">
                    <div className="col d-flex justify-content-center">
                        <input type="file" class="invisible" onChange={this.fileSelectedHandler} ref={fileInput => this.fileInput = fileInput} />
                        {actionButton}
                    </div>
                </div>
            </div>
        );
    }

    removePhotoClick = () => {
        const fd = new FormData();
        fd.append("PhotoNumber", this.props.photoNumber);
        fd.append("CategoryId", this.props.categoryId);
        axios.post("../RemovePhoto", fd)
            .then(res => {
                if (res.data.success)
                    this.setState({ photoImage: [] });
            });
    }

    fileUploadIsValid = (event) => {
        let message = "";

        if (event.target.files.length > 1)
            message = "Don't be silly. Only one photo can be selected at a time'";

        if (message == "" && !event.target.files[0].type.startsWith("image/"))
            message = "You are a dummy. Only image type files can be selected'";

        if (message != "")
            alert(message);

        return (message == "");
    }

    fileSelectedHandler = (event) => {
        if (event.target.files.length > 0 && this.fileUploadIsValid(event)) {
            let filetype = event.target.files[0].type;
            let filename = event.target.files[0].name;

            const fd = new FormData();
            fd.append("FileContents", event.target.files[0], filename);
            fd.append("FileType", filetype);
            fd.append("PhotoNumber", this.props.photoNumber);
            fd.append("CategoryId", this.props.categoryId);
            axios.post("../SubmitPhoto", fd, {
                onUploadProgress: progressEvent => {
                    console.log("Upload progress: " + Math.round((progressEvent.loaded / progressEvent.total) * 100));
                    this.setState({ uploadProgress: Math.round((progressEvent.loaded / progressEvent.total) * 100) });
                }
            })
                .then(res => {
                    if (res.data.success) {
                        this.setState({ photoImage: res.data.result, uploadProgress: 0 });
                    }
                    else {
                        this.setState({ uploadProgress: 0 });
                        alert(res.data.errorMessage);
                    }
                });
        }
    }
}

class UploadImageArea extends React.Component {
    // Use the render function to return JSX component
    render() {
        return (
            <div>
                <div className="row justify-content-around">
                    <UploadImageComponent photoNumber="1" categoryId={this.props.categoryId} />
                    <UploadImageComponent photoNumber="2" categoryId={this.props.categoryId} />
                </div>
            </div>
        );
    }
}

