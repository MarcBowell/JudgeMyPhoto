﻿// Obtain the root
const rootElement = document.getElementById('root')

// Create a ES6 class components
class ImageComponent extends React.Component {
    render() {
        let photoUrl = `../GetFullPhoto?cId=${this.props.categoryId}&pId=${this.props.photoId}`;
        let photoClass = "photo-small ";
        if (this.props.orientation == "L")
            photoClass = photoClass + "landscape-image";
        else
            photoClass = photoClass + "portrait-image";
        return (
            <img class={photoClass} src={photoUrl} onClick={(e) => this.photoHandleClick(e, this.props.photoId)}></img>
        )
    };

    photoHandleClick = (e, pId) => {
        e.stopPropagation();
        this.props.selectPhotoClick(pId);
    }
}

const ViewTypes = Object.freeze({ "slideShow": 1, "photoList": 2 });
class ImageViewingArea extends React.Component {
    constructor(props) {
        super(props);
        this.state = { photos: [], viewType: ViewTypes.photoList, activeSlideshowPhoto: 0 };
        this.getPhotosFromServer();
    }

    getPhotosFromServer = () => {
        let url = `../GetPhotosForCategory/${this.props.categoryId}`;
        fetch(url)
            .then((response) => {
                return response.json();
            })
            .then((data) => {
                this.setState({ photos: data });
            });
    }

    selectPhotoClick = (photoId) => {
        this.setState({ activeSlideshowPhoto: photoId, viewType: ViewTypes.slideShow });
    }

    renderPhotoList = () => {
        const items = this.state.photos.map((item) =>
            <ImageComponent categoryId={this.props.categoryId} photoId={item.photoId} orientation={item.orientation} selectPhotoClick={this.selectPhotoClick} />
        );
        return (
            <div>
                <nav class="navbar nav-tabs ml-auto w-100 justify-content-end">                    
                    <button type="button" class="btn photo-viewer-nav-button" onClick={this.slideshowButtonClick}>Slideshow</button>
                    <button type="button" class="btn photo-viewer-nav-button" onClick={this.getPhotosFromServer}>Shuffle</button>
                </nav>
                <div class="small-photo-container">
                    {items}
                </div>
            </div>
        );
    }

    slideshowButtonClick = () => {
        this.setState({ activeSlideshowPhoto: 0, viewType: ViewTypes.slideShow });
    }

    carouselCloseButtonClick = () => {
        this.setState({ viewType: ViewTypes.photoList });
    }

    renderSlideShow = () => {
        return (
            <PhotoCarousel categoryId={this.props.categoryId} photos={this.state.photos} activePhoto={this.state.activeSlideshowPhoto} closeButtonClick={this.carouselCloseButtonClick} />
        );
    }

    render() {
        if (this.state.viewType == ViewTypes.photoList)
            return this.renderPhotoList();
        else
            return this.renderSlideShow();
    }
}

class PhotoCarousel extends React.Component {
    closeButtonClick = (e) => {
        e.stopPropagation();
        this.props.closeButtonClick();
    }

    render() {
        const items = this.props.photos.map((item, index) => {
            const activeValue = ((index == 0 && this.props.activePhoto == 0) ||
                (item.photoId == this.props.activePhoto)
                ? "true" : "false");
            return (
                <PhotoCarouselItem categoryId={this.props.categoryId} photoId={item.photoId} title={item.title} active={activeValue} />
            )
        });

        return (
            <div id="myCarousel" class="carousel slide" data-ride="carousel">
                <div class="carousel-inner" role="listbox">
                    {items}
                    <button type="button" class="close" aria-label="Close" onClick={(e) => this.closeButtonClick(e)}>
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <a class="carousel-control-prev" href="#myCarousel" role="button" data-slide="prev">
                        <span class="carousel-control-prev-icon"></span>
                        <span class="sr-only">Previous</span>
                    </a>
                    <a class="carousel-control-next" href="#myCarousel" role="button" data-slide="next">
                        <span class="carousel-control-next-icon"></span>
                        <span class="sr-only">Next</span>
                    </a>
                </div>
            </div>
        );
    }
}

class PhotoCarouselItem extends React.Component {
    render() {
        let photoUrl = `../GetFullPhoto?cId=${this.props.categoryId}&pId=${this.props.photoId}`;
        let itemClass = "carousel-item";
        if (this.props.active == "true")
            itemClass = itemClass + " active";
        return (
            <div class={itemClass}>
                <div class="carousel-photo-container">
                    <img class="carousel-photo-image" src={photoUrl} />
                </div>
                <div class="carousel-caption carousel-photo-name">
                    {this.props.title}
                </div>
            </div>
        );
    }
}