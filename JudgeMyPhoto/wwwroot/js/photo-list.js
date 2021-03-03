// Obtain the root
const rootElement = document.getElementById('root')

// Create a ES6 class components
class ImageComponent extends React.Component {
    render() {
        let photoUrl = `../GetFullPhoto?cId=${this.props.categoryId}&pId=${this.props.photoId}`;
        let photoClass = "photo-small ";
        if (this.props.orientation == "L")
            photoClass = photoClass + "landscape-image ";
        else
            photoClass = photoClass + "portrait-image ";
        if (this.props.votingPosition > 0 && this.props.viewType == ViewTypes.votingMode)
            photoClass = photoClass + "photo-provisional-voted";        

        return (
            <div class="image-container">
                <img class={photoClass} src={photoUrl} onClick={(e) => this.photoHandleClick(e, this.props.photoId)}></img>
                {this.renderPhotoText()}
            </div>
            
        )
    };

    renderPhotoText = () => {
        if (this.props.viewType == ViewTypes.votingMode && this.props.votingPosition > 0) {
            return (<div class="image-text">{this.props.votingPosition}</div>);
        }
        else {
            return ("");
        }
    }


    photoHandleClick = (e, pId) => {
        e.stopPropagation();
        this.props.selectPhotoClick(pId);
    }
}

const StatusTypes = Object.freeze({ "judging": "OJ", "completed": "CM" });
const ViewTypes = Object.freeze({ "slideShow": 1, "photoList": 2, "votingMode": 3 });
class ImageViewingArea extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            photos: [],
            viewType: ViewTypes.photoList,
            activeSlideshowPhoto: 0,
            currentVoteIndex: 0,
            provisionalVotes: [0, 0, 0],
            lastExceptionMessage: "",
            existingVotes: [],
            scoreboard: []
        };
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

        if (this.props.categoryStatus == StatusTypes.judging) {
            let votesUrl = `../GetExistingVotes/${this.props.categoryId}`;
            fetch(votesUrl)
                .then((response) => {
                    return response.json();
                })
                .then((data) => {
                    this.setState({ existingVotes: data.result });
                });
        }
    }

    selectPhotoClick = (photoId) => {
        if (this.state.viewType == ViewTypes.photoList) {
            this.setState({ activeSlideshowPhoto: photoId, viewType: ViewTypes.slideShow });
        }
        else {
            let provisionalVotes = this.state.provisionalVotes.map((x) => x);
            provisionalVotes[this.state.currentVoteIndex] = photoId;
            let votedIndex = this.state.currentVoteIndex + 1;         
            this.setState({ currentVoteIndex: votedIndex, provisionalVotes: provisionalVotes});
            if (votedIndex > 2)
                this.commitPhotoVote();
        }
    }

    commitPhotoVote = () => {
        $("#voting-save-confirmation-dialog").modal('show');        
    }

    getProvisionalVotingFor = (itemId) => {
        return this.state.provisionalVotes.findIndex((x) => x == itemId) + 1;
    }

    renderPhotoList = () => {
        const items = this.state.photos.map((item) =>
            <ImageComponent categoryId={this.props.categoryId} photoId={item.photoId} orientation={item.orientation} selectPhotoClick={this.selectPhotoClick} votingPosition={this.viewType == ViewTypes.photoList ? 0 : this.getProvisionalVotingFor(item.photoId)} viewType={this.state.viewType} />
        );
        return (
            <div>
                {this.state.viewType == ViewTypes.votingMode ? this.renderVotingListMenu() : this.renderPhotoListMenu() }
                <div class="small-photo-container">
                    {items}
                </div>
                {this.renderMessageBoxDialogs()}
            </div>
        );
    }

    renderPhotoListMenu = () => {
        return (
            <nav class="navbar nav-tabs ml-auto w-100 justify-content-end">
                {this.renderVotingButton()}
                <button type="button" class="btn photo-viewer-nav-button" onClick={this.slideshowButtonClick}>Slideshow</button>
                <button type="button" class="btn photo-viewer-nav-button" onClick={this.getPhotosFromServer}>Shuffle</button>                
            </nav>                
        );
    }

    renderVotingButton = () => {
        if (this.props.categoryStatus == StatusTypes.judging) {
            if (this.state.existingVotes.length > 0) {
                return (<button type="button" class="btn photo-viewer-nav-button" data-toggle="modal" data-target="#myvotes-dialog">My votes</button>);
            }
            else {
                return (<button type="button" class="btn photo-viewer-nav-button" onClick={this.startVotingButtonClick}>Vote</button>);
            }
        }

        if (this.props.categoryStatus == StatusTypes.completed) {
            return (<button type="button" class="btn photo-viewer-nav-button" onClick={this.showScoreboardButtonClick}>Scoreboard</button>);
        }
    }

    renderMessageBoxDialogs = () => {
        return (
            <div>
                {this.renderVotingInstructionsDialog()}
                {this.renderVotingSaveConfirmationDialog()}
                {this.renderVotingSuccessConfirmationDialog()}
                {this.renderExceptionMessageDialog()}
                {this.renderMyVotesDialog()}
                {this.renderScoreboardDialog()}
            </div>
        );
    }

    renderVotingListMenu = () => {
        return (
            <nav class="navbar nav-tabs ml-auto w-100 justify-content-end">
                <button type="button" class="btn photo-viewer-nav-button" onClick={this.cancelVotingButtonClick}>Cancel vote</button>
            </nav>);
    }

    renderMyVotesDialog = () => {
        return (
            <div class="modal fade" id="myvotes-dialog" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="myvotes-title">My votes</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            You have voted for photos:<br/>
                            {this.state.existingVotes.map((item, index) => <div>{index + 1}. {item}<br/></div>) }
                            <p>Would you like to change these votes?</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" data-dismiss="modal" onClick={this.mvVotesYesButtonClick}>Yes</button>
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>
                        </div>
                    </div>
                </div>
            </div>);
    }

    renderVotingInstructionsDialog = () => {
        return (
            <div class="modal fade" id="voting-instructions-dialog" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="voting-instructions-title">Voting instructions</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Fair dinkum. To vote for your favourite photos, click on each selected photo in turn from first to third favourite photos. Once saved, you can change your votes at any time by voting again.
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" data-dismiss="modal" onClick={this.commenceVotingButtonClick}>OK</button>
                        </div>
                    </div>
                </div>
            </div>);
    }

    renderVotingSaveConfirmationDialog = () => {
        return (
            <div class="modal fade" id="voting-save-confirmation-dialog" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="voting-save-confirmation-title">Voting confirmation</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Strewth, that's a good selection. Do you want to save these votes?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" data-dismiss="modal" onClick={this.saveVotingYesButtonClick}>Yes</button>
                            <button type="button" class="btn btn-secondary" data-dismiss="modal" onClick={this.saveVotingNoButtonClick}>No</button>
                        </div>
                    </div>
                </div>
            </div>);
    }

    renderVotingSuccessConfirmationDialog = () => {
        return (
            <div class="modal fade" id="voting-success-confirmation-dialog" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="voting-success-confirmation-title">Voting update</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Bonzo! Your votes have been saved. Have a good day.
                        </div>
                        <div class="modal-footer">                            
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">OK</button>
                        </div>
                    </div>
                </div>
            </div>);
    }

    renderScoreboardDialog = () => {
        return (
            <div class="modal fade" id="scoreboard-dialog" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="scoreboard-dialog-title">Voting scoreboard</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            {this.state.scoreboard.map((item) => this.renderScoreboardDialogItem(item))}
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">OK</button>
                        </div>
                    </div>
                </div>
            </div>);
    }

    renderScoreboardDialogItem = (item) => {
        return (
            <div class="row">                
                <div class="col">{item.position} - {item.photoName}</div>
                <div class="col">{item.points} points</div>
            </div>
            )
    }

    renderExceptionMessageDialog = () => {
        return (
            <div class="modal fade" id="exception-message-dialog" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exception-message-title">You have an error</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Oops. An error has occurred. {this.state.lastExceptionMessage}.
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Ok</button>
                        </div>
                    </div>
                </div>
            </div>);
    }

    startVotingButtonClick = () => {
        $("#voting-instructions-dialog").modal('show');
    }

    mvVotesYesButtonClick = () => {
        this.startVotingButtonClick();
    }

    commenceVotingButtonClick = () => {
        this.setState({ viewType: ViewTypes.votingMode, currentVoteIndex: 0, provisionalVotes: [0, 0, 0] });
    }

    saveVotingYesButtonClick = () => {
        this.setState({ viewType: ViewTypes.photoList });
        this.postDataToServer("../SubmitVotes", { CategoryId: parseInt(this.props.categoryId), PhotoIds: this.state.provisionalVotes })
            .then((data) => {
                if (data.success) {
                    this.setState({ existingVotes: data.result });
                    $("#voting-success-confirmation-dialog").modal('show');
                }
                else {
                    this.setState({ lastExceptionMessage: data.errorMessage + ". Please try to vote again or contact your system administrator" });
                    $("#exception-message-dialog").modal('show');                    
                }                
            });
    }

    saveVotingNoButtonClick = () => {
        this.commenceVotingButtonClick();
    }

    cancelVotingButtonClick = () => {
        this.setState({ viewType: ViewTypes.photoList });
    }

    async postDataToServer(url, data) {
        // Default options are marked with *
        const response = await fetch(url, {
            method: 'POST',
            mode: 'same-origin', // no-cors, *cors, same-origin
            cache: 'no-store', // *default, no-cache, reload, force-cache, only-if-cached
            credentials: 'include', // include, *same-origin, omit
            headers: {
                'Content-Type': 'application/json'
                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            redirect: 'error', 
            referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
            body: JSON.stringify(data) // body data type must match "Content-Type" header
        });
        return response.json(); // parses JSON response into native JavaScript objects
    }

    showScoreboardButtonClick = () => {
        this.setState({ scoreboard: [{ position: "", points: "", photoName: "Loading..."}] });
        $('#scoreboard-dialog').modal('show');

        const url = `../GetPhotoScoreboard/${this.props.categoryId}`;
        fetch(url)
            .then((response) => response.json())
            .then((data) => {
                if (data.success) {
                    this.setState({ scoreboard: data.result });
                }
                else {
                    $('#scoreboard-dialog').modal('hide');
                    this.setState({ lastExceptionMessage: data.errorMessage });
                    $("#exception-message-dialog").modal('show');
                }
            })
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
        if (this.state.viewType == ViewTypes.photoList || this.state.viewType == ViewTypes.votingMode)
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