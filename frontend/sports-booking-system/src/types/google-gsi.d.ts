declare global {
  interface GoogleCredentialResponse {
    credential: string;
    select_by?: string;
    clientId?: string;
  }

  interface GoogleIdInitConfig {
    client_id: string;
    callback: (response: GoogleCredentialResponse) => void;
    auto_select?: boolean;
    cancel_on_tap_outside?: boolean;
    ux_mode?: 'popup' | 'redirect';
  }

  interface GoogleIdButtonConfig {
    theme?: 'outline' | 'filled_blue' | 'filled_black';
    size?: 'small' | 'medium' | 'large';
    type?: 'standard' | 'icon';
    text?: 'signin_with' | 'signup_with' | 'continue_with' | 'signin';
    shape?: 'rectangular' | 'pill' | 'circle' | 'square';
    width?: number | string;
    logo_alignment?: 'left' | 'center';
    locale?: string;
  }

  const google: {
    accounts: {
      id: {
        initialize(config: GoogleIdInitConfig): void;
        renderButton(parent: HTMLElement, config: GoogleIdButtonConfig): void;
        prompt(): void;
        cancel(): void;
        disableAutoSelect(): void;
      };
    };
  };
}

export {};
